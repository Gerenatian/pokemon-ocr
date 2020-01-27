using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pokemon_ocr
{
    class Program
    {
        public static string strRegex = @"[0-9]+\/([0-9]+)";

        static void Main(string[] args)
        {
            var Ocr = new IronOcr.AutoOcr();
            var Result = Ocr.Read(@"C:\workspaces\pokemon-ocr\pokemon-ocr\tests\test.jpg");
            Console.WriteLine(Result.Text);


            Regex myRegex = new Regex(strRegex);
            Match myMatch = myRegex.Match(Result.Text);


            Console.WriteLine(myMatch);

            Console.Write("yo");
        }
    }
}

//