namespace ForesterAPI.Models
{
    public struct Application
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string quickDescription { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string isPrivate { get; set; }
        public ulong mainDeveloper { get; set; }
        public string absoluteUpdate { get; set; }
        public int downloadNumber { get; set; }
    }
}
