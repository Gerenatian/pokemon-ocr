using Newtonsoft.Json;
using pokemon_ocr.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pokemon_ocr
{
    class Program
    {
        public static string strRegex = @"[0-9]+\/([0-9]+)";
        public static string set = "sm12";
        private static readonly HttpClient client = new HttpClient();
        public static string RestBase = "http://pokemontwitchrest.azurewebsites.net/api/chat";
        //public static string RestBase = "https://localhost:44396/api/chat";


        static void Main(string[] args)
        {
            var Ocr = new IronOcr.AutoOcr();
            var Result = Ocr.Read(@"C:\workspaces\pokemon-ocr\pokemon-ocr\tests\test.jpg");
            Console.WriteLine(Result.Text);


            Regex myRegex = new Regex(strRegex);
            Match myMatch = myRegex.Match(Result.Text);
            Card card = new Card();

            string searchId = set + "-" + myMatch.ToString().Split('/')[0];

            using (WebResponse response = WebRequest.Create("https://api.pokemontcg.io/v1/cards/" + searchId).GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    var cardJson = sr.ReadToEnd();

                    var cards = JsonConvert.DeserializeObject<Cards>(cardJson);

                    card = cards.card;

                }
            }

            InsertCardAsync(card);

            Console.Write("yo");
        }

        public static async void InsertCardAsync(Card card)
        {
            WebRequest request = WebRequest.Create(RestBase + @"/Inventory");
            request.Method = "POST";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(card);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}

//