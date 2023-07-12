using System.ComponentModel.DataAnnotations;

namespace ForesterAPI.Models
{
    public struct Application
    {
        public int ID { get; set; }
        public int Owner { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public bool IsPrivate { get; set; }
        public int DownloadNumber { get; set; }
    }
}
