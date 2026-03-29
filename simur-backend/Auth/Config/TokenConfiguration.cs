namespace simur_backend.Auth.Config
{
    public class TokenConfiguration
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int RefreshLifetimeMinutes { get; set; }
        public int TokenLifetimeMinutes { get; set; }
    }
}
