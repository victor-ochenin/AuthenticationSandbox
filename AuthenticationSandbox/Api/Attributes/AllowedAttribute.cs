namespace AuthenticationSandbox.Api.Attributes
{
    // AllowedAttribute - атрибут - для разрешения доступа к обработчику api без требования аутентификации
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowedAttribute : Attribute
    {
        private readonly bool _adminOnly;

        public bool AdminOnly { get => _adminOnly; }

        public AllowedAttribute(bool adminOnly=false) {
            _adminOnly = adminOnly;
        }
    }
}
