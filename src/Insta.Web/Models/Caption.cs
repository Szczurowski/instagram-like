using System;

namespace Insta.Web.Models
{
    [Serializable]
    public class Caption
    {
        public string Text { get; set; }

        public decimal Confidence { get; set; }
    }
}
