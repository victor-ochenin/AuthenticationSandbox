using AuthenticationSandbox.Api.Attributes;
using AuthenticationSandbox.Api.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSandbox.Api.Controller
{
    [Route("api")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        [Allowed]
        public StringMessage GetStatus()
        {
            return new StringMessage("server is running");
        }

        [HttpGet("ping")]
        [Allowed]
        public StringMessage Ping()
        {
            return new StringMessage("pong");
        }

        [HttpGet("protected")]
        public StringMessage ProtectedGetStatus()
        {
            return new StringMessage("[protected] server is running");
        }

        [HttpGet("protected/ping")]
        public StringMessage ProtectedPing()
        {
            return new StringMessage("[protected] pong");
        }
    }
}
