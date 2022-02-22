namespace ForesterAPI.Models.API
{
    public struct LoginToken
    {
        public ulong ID { get; set; }
        public string username { get; set; }
        public int exp { get; set; }
    }
}
