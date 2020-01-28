using Emgu.CV;
using IronOcr;
using Newtonsoft.Json;
using pokemon_ocr.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace pokemon_ocr
{
    class Program
    {
        public static string strRegex = @"[0-9]+\/([0-9]+)";
        public static string set = "sm12";
        private static readonly HttpClient client = new HttpClient();
        public static string tempImagesDir = @"C:\workspaces\pokemon-ocr\pokemon-ocr\tests";
        public static string RestBase = "http://pokemontwitchrest.azurewebsites.net/api/chat";
        //public static string RestBase = "https://localhost:44396/api/chat";

        static void Main(string[] args)
        {

            var searchId = CaptureCard();

            searchId = string.Format("{0}-{1}", set, searchId);

            Card card = new Card();

            using (WebResponse response = WebRequest.Create("https://api.pokemontcg.io/v1/cards/" + searchId).GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    var cardJson = sr.ReadToEnd();

                    var cards = JsonConvert.DeserializeObject<Cards>(cardJson);

                    card = cards.card;

                    InsertCardAsync(card);
                }
            }


            Console.Write("yo");
        }

        private static string CaptureCard()
        {
            bool cardRecognized = false;
            string id = string.Empty;
            VideoCapture capture = new VideoCapture();
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Autofocus, 1);
            capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30);

            while (!cardRecognized)
            {


                Thread.Sleep(2000);
                Bitmap image = capture.QueryFrame().Bitmap;

                saveJpg(image);
                var Ocr = new AdvancedOcr()
                {
                    CleanBackgroundNoise = true,
                    EnhanceContrast = true,
                    EnhanceResolution = true,
                    Language = IronOcr.Languages.English.OcrLanguagePack,
                    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Fast,
                    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                    DetectWhiteTextOnDarkBackgrounds = false,
                    InputImageType = AdvancedOcr.InputTypes.Snippet,
                    RotateAndStraighten = true,
                    ColorDepth = 0
                };

                var Result = Ocr.Read(image);
                Console.WriteLine(Result.Text);

                Regex myRegex = new Regex(strRegex);
                Match myMatch = myRegex.Match(Result.Text);

                Console.WriteLine(Result.Text);
                if (String.IsNullOrEmpty(myMatch.ToString().Split('/')[0]))
                {
                    continue;
                }
                id = myMatch.ToString().Split('/')[0];
                break;
            }

            return id;
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

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private static void saveJpg(Bitmap myBitmap)
        {
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.

            // An EncoderParameters object has an array of EncoderParameter

            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with quality level 100.
            myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            myBitmap.Save(tempImagesDir + @"\captureAttempt.jpg", myImageCodecInfo, myEncoderParameters);
        }
    }
}

//