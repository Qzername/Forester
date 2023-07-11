namespace Forester.Models.API
{
    public struct Account
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDeveloper { get; set; }
    }
}
