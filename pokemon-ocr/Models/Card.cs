using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokemon_ocr.Models
{
    public class Card
    {
        public string id { get; set; }
        public string imageUrl { get; set; }

        public string name { get; set; }

        public string set { get; set; }
        public string series { get; set; }

        public int nationalPokedexNumber { get; set; }

        public string imageUrlHiRes { get; set; }

        public string rarity { get; set; }

        public string setCode { get; set; }
    }
}
