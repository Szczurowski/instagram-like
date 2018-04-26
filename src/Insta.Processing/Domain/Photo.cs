namespace Insta.Processing.Domain
{
    public class Photo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string OriginalLocation { get; set; }

        public string ThumbnailLocation { get; set; }

        public string VisionAnalysis { get; set; }
    }
}
