using AuthenticationSandbox.Api.Attributes;
using AuthenticationSandbox.Api.Messages;
using AuthenticationSandbox.Model.User;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AuthenticationSandbox.Api.Middleware
{
    // SecurityMiddleware - middleware для ограничения доступа к ресурсам приложения
    // реализует аутентификацию пользователей через apiKey
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly UserService _userService;

        public SecurityMiddleware(RequestDelegate next, UserService userService) {
            _next = next;
            _userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            // ПРОВЕРИМ, А НУЖНО ЛИ ЗАЩИЩАТЬ ДОСТУП ДЛЯ ЗАПРОСА
            // ЕСЛИ У ЦЕЛЕВОГО ОБРАБОТЧИКА ЕСТЬ АТРИБУТ Allowed -> ТО ЗАЩИТА НЕ НУЖНА

            // 1) получить целевой обработчик - endpoint
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>()!.Endpoint;
            if (endpoint == null)
            {
                // если не найден endpoint - пусть ASP сам обработает
                await _next(context);
                return;
            }

            // 2) проверить наличие атрибута Allowed
            AllowedAttribute? attr = endpoint.Metadata.GetMetadata<AllowedAttribute>();
            if (attr != null && !attr.AdminOnly)
            {
                // у обработчика есть атрибут allowed и он не только для админов -> дать доступ
                await _next(context);
                return;
            }

            // 3) проверить - есть ли доступ только для админов
            bool adminOnly = attr != null;

            // ПРОВЕРИМ ДОСТУП
            // 1. достать значение заголовка XApiKey из запроса
            StringValues apiKeyValues = context.Request.Headers["X-Api-Key"];
            if (apiKeyValues.Count != 1 || string.IsNullOrEmpty(apiKeyValues[0]))
            {
                // 400
                ErrorMessage error = new ErrorMessage(Type: "InvalidApiKeyHeader", Message: "X-Api-Key header is invalid");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(error);
                return;
            }
            string apiKey = apiKeyValues[0]!;

            // 2. проверить есть ли пользователь с таким apiKey
            try
            {
                User user = await _userService.Get(apiKey);
                if (adminOnly && !user.IsAdmin)
                {
                    // должен быть админо но им не является
                    // 403
                    ErrorMessage error = new ErrorMessage(Type: "AccessDenied", Message: "Access Denied");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(error);
                    return;
                }
                // пользователь такой есть -> дать доступ
                await _next(context);
            }
            catch (UserNotFoundException ex)
            {
                // 401
                ErrorMessage error = new ErrorMessage(Type: "AccessDenied", Message: "Access Denied");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
