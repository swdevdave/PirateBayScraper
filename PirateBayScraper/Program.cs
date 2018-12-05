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
        static void Main(string[] args)
        {

            GetHtmlAsync();
            Console.ReadLine();
                          
        }

        private static async void GetHtmlAsync()
        {
            var vartempurl = "DCs%20Legends.of.Tomorrow";

            var innerUrl = vartempurl;
            var baseUrl = "https://thepiratebay.rocks/search/";
            var endUrl = "/1/99/0";
            var fullurl = baseUrl + innerUrl + endUrl;


            var httpClient = new HttpClient();
            var htmlBody = await httpClient.GetStringAsync(fullurl);

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
