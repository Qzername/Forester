namespace ForesterAPI.Models
{
    public struct UpdateCredentials
    {
        public string token { get; set; }
        public string friendlyUsername { get; set; }
        public string password { get; set; }
        public string description { get; set; }
    }
}
