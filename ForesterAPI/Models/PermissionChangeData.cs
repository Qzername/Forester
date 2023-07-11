namespace ForesterAPI.Models
{
    public struct PermissionChangeData
    {
        public string ApplicationName { get; set; }
        public string AccountLogin { get; set; }
        public Permission Permission { get; set; }
    }
}
