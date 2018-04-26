using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insta.Web.Models
{
    [Serializable]
    public class Face
    {
        public int Age { get; set; }

        public string Gender { get; set; }

        public int FaceRectange { get; set; }
    }
}
