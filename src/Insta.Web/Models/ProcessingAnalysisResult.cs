using System;
using System.Collections.Generic;

namespace Insta.Web.Models
{
    [Serializable]
    public class ProcessingAnalysisResult
    {
        public Description Description { get; set; }

        public IReadOnlyCollection<Face> Faces { get; set; }
    }
}
