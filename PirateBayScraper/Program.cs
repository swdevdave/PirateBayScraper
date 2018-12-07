using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
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
        static string fullURL;
        static string updatedName;
        static string updatedSeasonNumber;
        static int episodeNumber = 01;
        static string magenetUrlfolder;
        static int counter = 0;
        static int episodeCounter = 1;
        static string seasonEpisode;

        static List<string> namesOfFiles = new List<string>();
        static List<string> magenetUrl = new List<string>();
        static List<string> noFileCount = new List<string>();

        static void Main(string[] args)
        {
            checkIfDirExists();
            ConsoleQuiz();
            
            GetHtmlAsync();
            System.Threading.Thread.Sleep(500);

            FinishQuiz();
            Console.ReadLine();
        }

       
        private static void FinishQuiz()
        {
            Console.Clear();
            string totalCounter = "\nCreated " + counter + " Magnets\n\n";
            Console.WriteLine(totalCounter);
            Console.WriteLine("Press Any Key to Exit...");
            Console.Read();
            Environment.Exit(0);
            
        }

        static void checkIfDirExists()
        {
            string folderName = @"\PirateBayMagnets";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            magenetUrlfolder = path + folderName;
    
            Console.WriteLine(magenetUrlfolder.ToString());
            if (!Directory.Exists(magenetUrlfolder))
            {
                Directory.CreateDirectory(magenetUrlfolder);
            }
           
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
            seasonNumberResponse = Console.ReadLine(); // TODO: Needs Validation.

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



        private static void URLBuilder()
        {
            if (episodeNumber < 10)
            {
                string newEpisodeNumber  = "0" + episodeNumber.ToString();
                seasonEpisode = "s" + updatedSeasonNumber + "e" + newEpisodeNumber;
            } else {
                seasonEpisode = "s" + updatedSeasonNumber + "e" + episodeNumber;
            }

            fullURL = String.Empty;
            string innerUrl = updatedName  + seasonEpisode;
            string baseUrl = "https://thepiratebay.rocks/search/";
            string endUrl = "/1/99/0";
            fullURL = baseUrl + innerUrl + endUrl;
            Console.WriteLine("\n" + fullURL);
            
            
        }

        private static void InvalidSearch()
        {
            Console.Clear();
            Console.WriteLine("Unable to find specified Title, please try again\n\n");
            Console.WriteLine("Press Any Key to Exit...");
            Console.Read();
            Environment.Exit(0);
        }



        private static async void GetHtmlAsync()
        {
            while (episodeCounter >= 1)
            {

                URLBuilder();
                System.Threading.Thread.Sleep(200);
                var httpClient = new HttpClient();
                var htmlBody = await httpClient.GetStringAsync(fullURL);
                System.Threading.Thread.Sleep(200);
                var htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(htmlBody);
                System.Threading.Thread.Sleep(200);
                var ScraperHtml = htmlDocument.DocumentNode.Descendants("table") // Base HTML DATA
                     .Where(node => node.GetAttributeValue("id", "")
                     .Equals("searchResult")).ToList();
                System.Threading.Thread.Sleep(200);
                var ScraperNames = ScraperHtml[0].Descendants("div")            // PARSED NAMES
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("detName")).ToList();
                System.Threading.Thread.Sleep(200);
                var ScraperMagentsOuter = ScraperHtml[0].Descendants("a")            // PARSED MAGNET LINKS
                    .Where(node => node.GetAttributeValue("href", "")
                    .Contains("magnet")).ToList();
                System.Threading.Thread.Sleep(200);
                var ScraperMibSize = ScraperHtml[0].Descendants("font")            // PARSED MAGNET LINKS
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("detDesc")).ToList();
                System.Threading.Thread.Sleep(200);

                if (ScraperNames.Count <= 0)
                {
                    
                    //string showName = showNameResponse.Replace(" ", "_");
                    //string textFilePath = magenetUrlfolder + @"\" + showName + "_Magnets" + ".txt";
                    //StreamWriter sw = new StreamWriter(textFilePath);               
                    //sw.Write("No Data Found for Episode: " + episodeNumber);
                    //sw.Close();
                    Console.WriteLine("No Data Found for Episode: " + episodeNumber);
                  
                    episodeNumber++;
                }
                else{
                    episodeNumber++;

                    foreach (var ScraperName in ScraperNames)           // Loop for Name of File
                    {
                        if (ScraperName.InnerText.Length == 0 && counter == 0)
                        {
                            //InvalidSearch();
                            Console.WriteLine("Invalid Search");

                        }
                                                
                        episodeCounter = ScraperName.InnerText.Length;
                        counter++;
                        namesOfFiles.Add(ScraperName.InnerText);

                    }
                    
                    Console.WriteLine(namesOfFiles[0].Trim() +"\n\n");
                    Console.WriteLine();

                    foreach (var ScraperMagnet in ScraperMagentsOuter)  // Loop for Magent
                    {
                        
                        string magent = ScraperMagnet.OuterHtml;
                        string[] trimed = magent.Split('"');

                        magenetUrl.Add(trimed[1]);
                    
                    }
                    
                    Console.WriteLine(magenetUrl[0]);
                    

                    //string showName = showNameResponse.Replace(" ", "_");
                    //string textFilePath = magenetUrlfolder + @"\" + showName + "_Magnets" + ".txt";
                    //StreamWriter sw = new StreamWriter(textFilePath);
                    //sw.Write(magenetUrl[0]);
                    //sw.Write(namesOfFiles[0]);
                    //sw.Close();
                    magenetUrl.Clear();
                    namesOfFiles.Clear();

                    Console.WriteLine();
                }
            }            
        }
    }
}
