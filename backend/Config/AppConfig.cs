namespace CrossyRoadApi.Config;

public class AppConfig
{
    public DatabaseConfig Database { get; set; }
    public OAuthConfig BoschOAuth { get; set; }
    public OAuthConfig MicrosoftOAuth { get; set; }
    public string Domain { get; set; }
}