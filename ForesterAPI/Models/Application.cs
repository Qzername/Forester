namespace ForesterAPI.Models
{
    public struct Application
    {
        public int ID;
        public int Owner;
        public string Name;
        public string ShortDescription;
        public string Description;
        public string Version;
        public bool IsPrivate;
        public int DownloadNumber;
    }
}
