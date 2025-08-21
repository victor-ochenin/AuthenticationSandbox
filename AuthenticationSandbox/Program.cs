using AuthenticationSandbox.Api.Middleware;
using AuthenticationSandbox.Model.Encoder;
using AuthenticationSandbox.Model.User;
using AuthenticationSandbox.Stub;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
builder.Services.AddSingleton<IEncoder, MD5>();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<SecurityMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var service = scope.ServiceProvider.GetRequiredService<UserService>();
    if (await users.CountAdmins() == 0)
    {
        string adminApiKey = await service.Register("admin", true);
        Console.WriteLine("admin has been created successfully, admin api key: " + adminApiKey);
    }
}

app.Run();
