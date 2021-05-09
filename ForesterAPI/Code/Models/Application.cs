namespace ForesterAPI.Models
{
    public struct Application
    {
        public string name;
        public Account author;
        public string isPrivate;
        public Account[] allowedAccounts;
        public string isInDownloadFolder;
    }
}
