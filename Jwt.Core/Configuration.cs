namespace Jwt.Core;
public static class Configuration
{
    public static SecretsConfiguration Secrets { get; set; } = new();
    public static EmailConfiguration Email { get; set; } = new();
    public static SendGridConfiguration SendGrid { get; set; } = new();
    public static DatabaseConfiguration Database { get; set; } = new();

    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class EmailConfiguration
    {
        public string DefaultFromEmail { get; set; } = "thiago@teste.com.br";
        public string DefaultFromName { get; set; } = "thiago-teste";
    }

    public class SendGridConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
    }

    #region Usado para gerar a senha do usuário
    public class SecretsConfiguration
    {
        public string ApiKey { get; set; } = string.Empty;
        public string JwtPrivateKey { get; set; } = string.Empty;
        public string PasswordSaltKey { get; set; } = string.Empty;
    #endregion
    }
}
