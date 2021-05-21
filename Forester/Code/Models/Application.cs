namespace Forester.Models
{
    public struct Application
    {
        public string name { get; set; }
        public Account author { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string isPrivate { get; set; }
        public Account[] allowedAccounts { get; set; }
        public string isInDownloadFolder { get; set; }
    }
}
