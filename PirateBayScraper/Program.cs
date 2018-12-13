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
        static int noResponseCount = 0;
        static int noResponseTracker = 0;
        static int fileCreatedCount = 0;
        static bool firstRun = true;
        static bool noHD;
        static int noResponseHDCounter = 0;
        static string fullHDURL;
        static string noHDURL;

        static List<string> filesNotFoundNames = new List<string>();
        static List<string> namesOfFiles = new List<string>();
        static List<string> magenetUrl = new List<string>();
        static List<string> noFileCount = new List<string>();
        static List<string> parsedResponses = new List<string>();

        

        static void Main(string[] args)
        {
            checkIfDirExists();
            ConsoleQuiz();

            GetHtmlAsync();
            System.Threading.Thread.Sleep(500);

            Console.Read();
        }

        private static void FinishQuiz()
        {

            string totalCounter = "\nCreated " + fileCreatedCount + " Magnets\n\n";

            Console.WriteLine(totalCounter);
            for (int i = 0; i < filesNotFoundNames.Count -5; i++)               // -5 is used becuse 5 is the end counter for loop
            {
                Console.WriteLine("Unable to find Episode: " + (Int16.Parse(filesNotFoundNames[i])-1) + "\n");
            }
            Console.WriteLine("Press Any Key to Exit...");
            Console.Read();
            Environment.Exit(0);

        }

        static void checkIfDirExists()
        {
            string folderName = @"\PirateBayMagnets";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            magenetUrlfolder = path + folderName;

            if (!Directory.Exists(magenetUrlfolder))
            {
                Directory.CreateDirectory(magenetUrlfolder);
                Console.WriteLine("Created Folder: " + magenetUrlfolder.ToString());
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

            do
            {
                Console.Write("\nWhat season would you like?\n");
                seasonNumberResponse = Console.ReadLine();                                  
                int result;
                bool ifSucess = int.TryParse(seasonNumberResponse, out result);         // Begin Validation Logic
                if (ifSucess)
                {
                    if (result < 10)
                    {
                        updatedSeasonNumber = seasonNumberResponse.ToString();
                        updatedSeasonNumber = "0" + updatedSeasonNumber;
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("\nPlease enter a valid number.\n");
                }
                
            } while (true);


            do
            {
                Console.WriteLine("\nWould you like to search for HD versions?" );
                Console.Write("Y/N\n");

                hiDefResponse = Console.ReadLine();
                // Begin Validation Logic
                if (hiDefResponse.ToLower().Trim() == "y")
                {
                    hiDef = true;
                    Console.WriteLine("Searching for HiDef");
                    break;

                } else if (hiDefResponse.ToLower().Trim() == "n")
                {
                    hiDef = false;
                    Console.WriteLine("Searching for most results");
                    break;
                }
                Console.WriteLine("Please enter Y or N only");
                
            } while (true);

        }

        private static void URLBuilder()
        {
            if (episodeNumber < 10)
            {
                string newEpisodeNumber = "0" + episodeNumber.ToString();
                seasonEpisode = "s" + updatedSeasonNumber + "e" + newEpisodeNumber;
            }
            else
            {
                seasonEpisode = "s" + updatedSeasonNumber + "e" + episodeNumber;
            }

            fullURL = String.Empty;
            string innerUrl = updatedName + seasonEpisode;
            string baseUrl = "https://thepiratebay.rocks/search/";
            string endUrl = "/1/99/0";
            fullHDURL = baseUrl + innerUrl + "%20720p" + endUrl;
            fullURL = baseUrl + innerUrl + endUrl;
            
            
            //Console.WriteLine("\n" + fullURL);

        }

        static void WriteResponses()
        {
            string showName = showNameResponse.Replace(" ", "_");
            string textFilePath = String.Empty;

            if (hiDef)
            {
                textFilePath = magenetUrlfolder + @"\"
                + showName
                + "_magnets_HD_S" + updatedSeasonNumber + ".txt";
            }
            else
            {
                textFilePath = magenetUrlfolder + @"\"
                + showName
                + "_magnets_S" + updatedSeasonNumber + ".txt";
            }
            
            StreamWriter sw = new StreamWriter(textFilePath);
            Console.WriteLine("File written to: " + textFilePath);

            for (int i = 0; i < filesNotFoundNames.Count - 4; i++)               // -5 is used becuse 5 is the end counter for loop
            {
                sw.WriteLine("Unable to find Episode: " + (Int16.Parse(filesNotFoundNames[i]) - 1));
                sw.WriteLine("\n\n");
            }
            System.Threading.Thread.Sleep(100);

            for (int i = 0; i < parsedResponses.Count; i++)
            {
                sw.WriteLine(parsedResponses[i]);
                sw.WriteLine("\n\n");
            }

            System.Threading.Thread.Sleep(100);

            sw.Close();
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
            while (noResponseCount <= 5)
            {
                URLBuilder();
                System.Threading.Thread.Sleep(100);
                var httpClient = new HttpClient();
                string htmlBody; 

                if (hiDef)
                {
                    htmlBody = String.Empty;

                    if (noResponseHDCounter < 1)
                    {
                        
                        htmlBody = await httpClient.GetStringAsync(fullURL);
                    }
                    else
                    {
                        
                        htmlBody = await httpClient.GetStringAsync(noHDURL);
                        noHDURL = String.Empty;
                        noHD = true;
                        noResponseHDCounter = 0;
                        episodeNumber--;
                    }
                }
                else
                {
                    htmlBody = await httpClient.GetStringAsync(fullURL);
                }
                
                
                System.Threading.Thread.Sleep(100);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlBody);
                System.Threading.Thread.Sleep(100);

                var ScraperHtml = htmlDocument.DocumentNode.Descendants("table")    // Base HTML DATA
                        .Where(node => node.GetAttributeValue("id", "")
                        .Equals("searchResult")).ToList();
                System.Threading.Thread.Sleep(100);

                var ScraperNames = ScraperHtml[0].Descendants("div")                // PARSED NAMES
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("detName")).ToList();
                System.Threading.Thread.Sleep(100);

                var ScraperMagentsOuter = ScraperHtml[0].Descendants("a")            // PARSED MAGNET LINKS
                    .Where(node => node.GetAttributeValue("href", "")
                    .Contains("magnet")).ToList();
                System.Threading.Thread.Sleep(100);

                var ScraperMibSize = ScraperHtml[0].Descendants("font")             // PARSED MAGNET LINKS
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("detDesc")).ToList();
                System.Threading.Thread.Sleep(100);

                if (ScraperNames.Count <= 1)
                {
                    //Console.WriteLine("No Data Found for Episode: " + episodeNumber);
                    noResponseCount++;
                    episodeNumber++;
                    noResponseTracker++;
                    if (hiDef && noHD)
                    {
                        filesNotFoundNames.Add(episodeNumber.ToString());
                        noHD = false;
                    }
                    noHDURL = fullURL;
                    noResponseHDCounter++;
                }
                else
                {

                    noResponseCount = 0;
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
                    
                    //firstRunNameCheck();
                
                
                    

                    foreach (var ScraperMagnet in ScraperMagentsOuter)  // Loop for Magent
                    {

                        string magent = ScraperMagnet.OuterHtml;
                        string[] trimed = magent.Split('"');

                        magenetUrl.Add(trimed[1]);

                    }
                    parsedResponses.Add(magenetUrl[0]);

                    fileCreatedCount = parsedResponses.Count;
                    Console.Clear();
                    Console.WriteLine("\n Files found:" + fileCreatedCount + "\n");


                    magenetUrl.Clear();
                    namesOfFiles.Clear();

                }

            }
            Console.WriteLine("Finished pulling Files");
            System.Threading.Thread.Sleep(1000);
            Console.Clear();
            WriteResponses();
            System.Threading.Thread.Sleep(200);
            FinishQuiz();
        }

        // to be implemented later
        static void firstRunNameCheck()
        {
            if (firstRun)
            {
                Console.Clear();
                Console.WriteLine(namesOfFiles[0]);

                Console.WriteLine("Please verify name above pulled is correct before proceeding\n");
                Console.WriteLine("Y/N");
                do
                {
                    string nameVerification = Console.ReadLine();
                    // Begin Validation Logic
                    if (nameVerification.ToLower() == "y")
                    {
                        firstRun = false;
                        break;
                        
                    }else if (nameVerification.ToLower() == "n")
                    {
                        ConsoleQuiz();
                        magenetUrl.Clear();
                        namesOfFiles.Clear();
                        break;
                    }

                    Console.WriteLine("Please enter Y or N only");

                } while (true);
            }
            


        }
    }
}