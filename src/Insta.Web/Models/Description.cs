using System;
using System.Collections.Generic;

namespace Insta.Web.Models
{
    [Serializable]
    public class Description
    {
        public IReadOnlyCollection<string> Tags { get; set; }

        public IReadOnlyCollection<Caption> Captions { get; set; }
    }
}
