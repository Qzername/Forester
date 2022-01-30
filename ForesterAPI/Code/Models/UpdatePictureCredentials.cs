namespace ForesterAPI.Models
{
    public struct UpdatePictureCredentials
    {
        public string name { get; set; }
        public PictureManager.Picture pictureType { get; set; }
        public PictureManager.Folder objectType { get; set; }
        public Token token { get; set; }
    }
}
