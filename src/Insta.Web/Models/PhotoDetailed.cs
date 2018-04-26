using System;

namespace Insta.Web.Models
{
    [Serializable]
    public class PhotoDetailed
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string OriginalLocation { get; set; }

        public ProcessingAnalysisResult ProcessingAnalysisResult { get; set; }
    }
}
