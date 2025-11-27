namespace HouseBrokerApplication.API.Configurations.ConfigModels
{
    public class ClientSetting
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> AllowedScopes { get; set; } = new List<string>();
    }
}
