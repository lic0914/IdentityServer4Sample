namespace JwtBearer.Samples
{
    public class JwtSettings
    {
        public const string Key= "JwtSettings";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}