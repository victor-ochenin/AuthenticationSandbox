namespace AuthenticationSandbox.Api.Messages
{
    // StringMessage - строковое сообщение
    public record StringMessage(string Message);

    // ErrorMessage - сообщение об ошибке
    public record ErrorMessage(string Type, string Message);
}
