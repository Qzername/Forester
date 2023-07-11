namespace Forester.Models.API
{
    public struct PermissionChangeData
    {
        public string ApplicationName { get; set; }
        public string AccountLogin { get; set; }
        public Permission Permission { get; set; }
    }
}
