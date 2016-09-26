namespace WeddingApp
{
    /// <summary>
    /// Type shared by client and server, representing an image
    /// </summary>
    public class ImageInfo
    {
        public string key { get; set; }  // key used by server
        public string name { get; set; } // optional name provided by user
    }
}
