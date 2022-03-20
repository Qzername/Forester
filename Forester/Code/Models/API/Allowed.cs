namespace Forester.Models.API
{
    public struct Allowed
    {
        public string name { get; set; }
        public Account[] allowedUsers { get; set; }
        public Account[] allowedDevelopers { get; set; }
    }
}
