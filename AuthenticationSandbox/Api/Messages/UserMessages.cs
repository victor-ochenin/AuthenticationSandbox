namespace AuthenticationSandbox.Api.Messages
{
    public record LoginMessage(string Login);
    public record ApiKeyMessage(string ApiKey);
    public record AdminStatusMessage(string Login, bool IsAdmin);
}
