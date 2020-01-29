using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokemon_ocr.Models
{
    public class Opening
    {
        public string userId { get; set; }
        public string cardIds { get; set; }
        public decimal cost { get; set; }
        public bool isDevelopment { get; set; }
    }
}
