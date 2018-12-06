using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PirateBayScraper
{
    class Program
    {
        static string showNameResponse;
        static string seasonNumberResponse;
        static string hiDefResponse;
        static bool hiDef = true;
        static string vartempurl = "DCs%20Legends.of.Tomorrow";
        static string fullURL;
        static string updatedName;
        static string updatedSeasonNumber;
        static int episodesToParse;


        static void Main(string[] args)
        {
            ConsoleQuiz();
            URLBuilder();
            GetHtmlAsync();
            Console.ReadLine();
                          
        }

        private static void ConsoleQuiz()
        {
            Console.Write("Name of the Tv Show:\n");
            showNameResponse = Console.ReadLine().Trim();
            string[] splitName = showNameResponse.Split(' ');
        
            

            for (int i = 0; i < splitName.Length; i++)
            {
                updatedName += splitName[i] + "%20";
            }

            //Console.WriteLine(updatedName);
            int seasonNum;
            Console.Write("\nWhat Season Number?\n");
            seasonNumberResponse = Console.ReadLine();
            seasonNum = Int32.Parse(seasonNumberResponse);
            if (seasonNum < 10)
            {
                updatedSeasonNumber = seasonNumberResponse.ToString();
                updatedSeasonNumber = "0" + updatedSeasonNumber;
            }

            //Console.Write(updatedSeasonNumber);

            Console.Write("\nHD Versions? Y/N\n");
            hiDefResponse = Console.ReadLine();

            if (hiDefResponse.ToLower() != "y")
            {
                hiDef = false;
            }
            Console.WriteLine(hiDef);

        }



        static string URLBuilder()
        {

            string seasonEpisode = "S" + seasonNumberResponse + "E01";
            string innerUrl = updatedName  + seasonEpisode;
            string baseUrl = "https://thepiratebay.rocks/search/";
            string endUrl = "/1/99/0";
            fullURL = baseUrl + innerUrl + endUrl;
            Console.WriteLine("\n" + fullURL);

            return fullURL;
            
        }

        private static async void GetHtmlAsync()
        {
            

            var httpClient = new HttpClient();
            var htmlBody = await httpClient.GetStringAsync(fullURL);

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(htmlBody);

            var ScraperHtml = htmlDocument.DocumentNode.Descendants("table") // Base HTML DATA
                 .Where(node => node.GetAttributeValue("id", "")
                 .Equals("searchResult")).ToList();

            var ScraperNames = ScraperHtml[0].Descendants("div")            // PARSED NAMES
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("detName")).ToList();

            var ScraperMagentsOuter = ScraperHtml[0].Descendants("a")            // PARSED MAGNET LINKS
                .Where(node => node.GetAttributeValue("href", "")
                .Contains("magnet")).ToList();

            var ScraperMibSize = ScraperHtml[0].Descendants("font")            // PARSED MAGNET LINKS
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("detDesc")).ToList();


            List<string> names = new List<string>();

            foreach (var ScraperName in ScraperNames)
            {
                string nameList = ScraperName.InnerText.Trim('\n', '\t', '\r');
                //Console.WriteLine(nameList);
                //Console.WriteLine();


                names.Add(nameList);
            }
            Console.WriteLine(names[0]);
            Console.WriteLine();


            List<string> magnetList = new List<string>();

            foreach (var ScraperMagnet in ScraperMagentsOuter)
            {
                string magent = ScraperMagnet.OuterHtml;
                string[] trimed = magent.Split('"');

                magnetList.Add(trimed[1]);
                               
            
            }
            Console.WriteLine(magnetList[0]);

            foreach (var MibSize in ScraperMibSize)
            {
                Console.WriteLine();
            }



                                   
            Console.WriteLine();

        }
    }
}
