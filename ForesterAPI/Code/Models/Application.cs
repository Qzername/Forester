namespace ForesterAPI.Models
{
    public struct Application
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string quickDescription { get; set; }
        public string description { get; set; }
        public string profilePicture { get; set; }
        public string backgroundPicture { get; set; }
        public string version { get; set; }
        public bool isPrivate { get; set; }
        public ulong mainDeveloper { get; set; }
        public ulong[] allowedDevelopers { get; set; }
        public ulong[] allowedUsers { get; set; }
        public int downloadNumber { get; set; }
    }
}
