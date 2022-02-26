namespace Forester.Models.API
{
    /// <summary>
    /// Reprezentacja konta
    /// </summary>
    public struct Account
    {
        public ulong ID { get; set; } //ID kodowe np. 753344670448353353
        public int friendly_ID { get; set; } //przyjazne dla użytkownika id np. #6969
        public string friendlyUsername { get; set; } //nazwa użytkownika stosowana w aplikacji
        public string description { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isDeveloper { get; set; }
    }
}
