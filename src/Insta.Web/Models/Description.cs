using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insta.Web.Models
{
    [Serializable]
    public class Description
    {
        public IReadOnlyCollection<string> Tags { get; set; }

        public IReadOnlyCollection<Caption> Captions { get; set; }
    }
}
