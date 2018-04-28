using System.Net.Security;

namespace Insta.Processing.Domain
{
    public class Photo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] OriginalContent { get; set; }

        public byte[] ThumbnailContent { get; set; }

        public string VisionAnalysis { get; set; }
    }
}
