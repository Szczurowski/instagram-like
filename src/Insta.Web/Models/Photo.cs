using System;

namespace Insta.Web.Models
{
    [Serializable]
    public class Photo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailLocation { get; set; }
    }
}
