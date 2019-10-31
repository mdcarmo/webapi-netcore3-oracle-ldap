namespace $safeprojectname$.Domain.Infra.OracleObjects
{
    public class OracleSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string ServiceName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DirectoryServer { get; set; }
        public string DirectoryServerPort { get; set; }
        public string DefaultAdminContext { get; set; }
    }
}
