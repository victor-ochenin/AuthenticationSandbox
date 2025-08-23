using AuthenticationSandbox.Api.Attributes;
using AuthenticationSandbox.Api.Messages;
using AuthenticationSandbox.Model.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSandbox.Api.Controller
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        [Allowed(adminOnly: true)]
        public async Task<List<User>> GetAll()
        {
            return await _service.GetAll();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name ="X-Api-Key")] string apiKey)
        {
            try
            {
                // 200
                return Ok(await _service.Get(apiKey));
            }
            catch (UserNotFoundException ex)
            {
                // 404
                return NotFound(new ErrorMessage(Type: ex.GetType().Name, Message: ex.Message));
            }
        }

        [HttpPost]
        [Allowed]
        public async Task<IActionResult> Register(LoginMessage loginMessage)
        {
            try
            {
                string apiKey = await _service.Register(loginMessage.Login);
                // 200
                return Ok(new ApiKeyMessage(ApiKey: apiKey));
            }
            catch (InvalidLoginException ex)
            {
                // 400
                return BadRequest(new ErrorMessage(Type: ex.GetType().Name, Message: ex.Message));
            }
            catch (DuplicatedLoginException ex)
            {
                // 409
                return Conflict(new ErrorMessage(Type: ex.GetType().Name, Message: ex.Message));
            } 
        }

        [HttpPost("admin")]
        [Allowed(adminOnly: true)]
        public async Task<IActionResult> SetAdmin(AdminStatusMessage message)
        {
            try
            {
                await _service.SetAdmin(message.Login, message.IsAdmin);
                return Ok();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new ErrorMessage(Type: ex.GetType().Name, Message: ex.Message));
            }
            catch (LastAdminRemovalException ex)
            {
                return Conflict(new ErrorMessage(Type: ex.GetType().Name, Message: ex.Message));
            }
        }
    }
}
