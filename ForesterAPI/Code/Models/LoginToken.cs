namespace ForesterAPI.Models
{
    public struct LoginToken
    {
        public ulong ID { get; set; }
        public string username { get; set; }
        public int exp { get; set; }
    }
}
