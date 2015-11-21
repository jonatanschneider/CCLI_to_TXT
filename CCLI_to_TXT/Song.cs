using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace CCLI_to_TXT
{
    class Song
    {
        /// <summary>
        /// Creates a new Song by providing the Songname
        /// </summary>
        /// <param name="name">Songname</param>
        public Song(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// Creates a new Song by providing the CCLI-Number
        /// </summary>
        /// <param name="ccliNumber">CCLI-Number</param>
        public Song(int ccliNumber)
        {
            this.number = ccliNumber;
        }

        private int number;
        public int Number
        {
            get { return number; }
        }
        private string name;
        public string Name
        {
            get { return name; }
        }

        //General methods
        /// <summary>
        /// Formats the songname so it can be used in URL's
        /// </summary>
        /// <returns>Songname with "+" instead of spaces</returns>
        public string MakeNameWebSearchable()
        {
            string name = this.name.Replace(" ", "+");
            return name;
        }

        /// <summary>
        /// Checks for duplicates in the output file by checking CCLI-Number
        /// </summary>
        /// <param name="fileLines">Lines in output file</param>
        /// <returns>Song is a duplicate</returns>
        public bool IsSongNumberAlreadyExisting(string[] fileLines)
        {
            string ccli = this.number.ToString();
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i].StartsWith(ccli))
                {
                    return true;
                }
            }
            return false;
        }

        //Methods with number as input
        /// <summary>
        /// Calls SetSongname and OutputHandling.NewSong
        /// </summary>
        public void AddNewSongByNumber()
        {
            this.SetSongnameByNumber();
            if (this.name != "Error")
            {
                OutputHandling.NewSong(this.name, this.number);
            }
        }

        /// <summary>
        /// Sets the Songname by calling the needed Methods and Decode the HTML Substring
        /// </summary>
        public void SetSongnameByNumber()
        {
            string website = DownloadWebsiteByNumber();
            if (website != "noSongnameError")
            {
                int[] positions = GetSongnamePositionInWebsiteByNumber(website);
                //Decode HTML-Text to Plain-Text
                this.name = WebUtility.HtmlDecode(website.Substring(positions[0], positions[1]));
            }
            else
            {
                this.name = "Error";
            }
        }

        /// <summary>
        /// Downloads HTML-Text of searchresults by pasting the CCLI-Number into the URL
        /// </summary>
        /// <returns>Songname or "noSongnameError" as Songname</returns>
        public string DownloadWebsiteByNumber()
        {
            WebClient webclient = new System.Net.WebClient();
            try
            {
                string webdata = webclient.DownloadString("https://de.songselect.com/songs/" + this.number);
                return webdata;
            }

            catch (WebException e)
            {
                Console.WriteLine("Fehler bei Song {0}. Bitte Error-Logs überprüfen!", this.number);
                Errorlogs.SongnameNotFoundLog(e, this.number);
                string noSongname = "noSongnameError";
                return noSongname;
            }
        }

        /// <summary>
        /// Gets the Index of the Songname in the HTML-File
        /// </summary>
        /// <param name="website">String with HTML-Source of search results</param>
        /// <returns>Start Index and length of the Songname</returns>
        public int[] GetSongnamePositionInWebsiteByNumber(string website)
        {
            //Between these two tags is the Songname
            string startTag = "<title>";
            string endTag = "</title>";
            //Array for start value and length
            int[] startTagAndLength = new int[2];

            //Get Position of the songname
            int indexOfStartTag = website.IndexOf(startTag) + startTag.Length;
            int indexOfEndTag = website.IndexOf(endTag);
            int lengthBetweenTags = indexOfEndTag - indexOfStartTag;

            startTagAndLength[0] = indexOfStartTag;
            startTagAndLength[1] = lengthBetweenTags;

            return startTagAndLength;
        }

        //Methods with name as input
        /// <summary>
        /// Calls OutputHandlin.NewSong if Songname is no Error
        /// </summary>
        public void AddNewSongByName()
        {
            if (this.name != "Error")
            {
                OutputHandling.NewSong(this.name, this.number);
            }
        }

        /// <summary>
        /// Prints out three possible Songs and sets the correct Songname or "Error"
        /// </summary>
        public void ChooseCorrectSong()
        {
            List<string> songs = this.GetSongnames();
            for (int i = 0; i < 3; i++)
            {
                if (songs[i].Length < 50)
                {
                    Console.Write((i + 1) + " - ");
                    Console.Write(songs[i]);
                    Console.WriteLine();
                }
                
            }
            Console.WriteLine("Bitte den gewünschten Song auswählen mit 1,2 oder 3");
            try
            {
                int choice = Convert.ToInt16(Console.ReadLine());
                this.name = songs[choice - 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Fehler! Bitte nur 1,2 oder 3 eingeben!");
                Console.WriteLine("Song wird übersprungen! - Weitere Informationen im Errorlog");
                Errorlogs.InputNoCorrectNumberLog(e);
                this.name = "Error";
            }
            catch (FormatException f)
            {
                Console.WriteLine("Fehler! Bitte nur 1, 2 oder 3 eingben!");
                Console.WriteLine("Song wird übersprungen! - Weitere Informationen im Errorlog");
                Errorlogs.InputNotANumberLog(f);
                this.name = "Error";
            }
        }

        /// <summary>
        /// Gets three possible Songs from search results, already HTML Decoded
        /// </summary>
        /// <returns>List with three Songnames</returns>
        public List<string> GetSongnames()
        {
            List<string> songnames = new List<string>(2);

            string website = this.DownloadWebsiteByName();
            List<int> positions = this.GetSongnamePositionsInWebsite(website);
            this.SetNumberBySongname(website);

            for (int i = 0; i < 3; i++)
            {
                string name = WebUtility.HtmlDecode(website.Substring(positions[0], positions[1]));
                
                if(name.Length < 100)
                {
                    songnames.Add(name);
                }
                else
                {
                    songnames.Add("Error");
                }

                //Remove unncecessary old values
                int item1 = positions[0];
                int item2 = positions[1];
                positions.Remove(item1);
                positions.Remove(item2);
            }

            return songnames;
        }

        /// <summary>
        /// Sets the Songnumber for the Songname
        /// </summary>
        /// <param name="website">String with HTML-Source of search results</param>
        public void SetNumberBySongname(string website)
        {
            string startTag = "<div class=\"songTitle\"><a href=\"/songs/";
            string endTag = WebUtility.HtmlEncode(this.name)+ "</a></div>";

            //Get Position of the songname
            int ccliNumberLength = 7;
            int indexOfStartTag = website.IndexOf(startTag) + startTag.Length;
            string tempWebsite = website.Substring(indexOfStartTag);
            try {
                this.number = Convert.ToInt32(website.Substring(indexOfStartTag, ccliNumberLength));
            }
            catch(FormatException)
            {
                Console.WriteLine("Das ist bisher noch nicht implementiert! Bitte momentan nur CCLI-Nummern angeben!");
                Errorlogs.SearchBySongnameNotImplementedLog();
                /*
                string tempStartTag = "<h4>CCLI Liednummer</h4>";
                string startTag2 = "<div>";
                string endTag2 = "</div>";

                int tempIndexOfStartTag = website.IndexOf(tempStartTag) + tempStartTag.Length;
                string tempTempWebsite = website.Substring(tempIndexOfStartTag);

                int tempIndexOfStartTag2 = tempTempWebsite.IndexOf(startTag2) + startTag2.Length + tempIndexOfStartTag;
                string tempWebsite3 = website.Substring(tempIndexOfStartTag2, ccliNumberLength);

                this.number = Convert.ToInt32(tempWebsite3);
                */

            }
        }

        /// <summary>
        /// Downloads HTML-Text of searchresults by pasting the Songname into the URL
        /// </summary>
        /// <returns>Songname or "noSongnameError" as Songname</returns>
        public string DownloadWebsiteByName()
        {
            WebClient webclient = new System.Net.WebClient();
            try
            {
                string webdata = webclient.DownloadString("http://de.search.ccli.com/search/results?SearchTerm=" + this.MakeNameWebSearchable());
                return webdata;
            }
            catch (WebException e)
            {
                Console.WriteLine("Fehler bei Song {0}. Bitte Error-Logs überprüfen!", this.name);
                Errorlogs.SongNameNotFoundLog(e, this.name);
                string noSongname = "noSongnameError";
                return noSongname;
            }
        }

        /// <summary>
        /// Gets the Index of the Songtitle in the HTML-Source search results
        /// </summary>
        /// <param name="website">String with HTML-Source of search results</param>
        /// <returns>Indexes of Songname and length</returns>
        public List<int> GetSongnamePositionsInWebsite(string website)
        {
            List<int> positions = new List<int>(2);
            string beginOfStartPattern = "<div class=\"songTitle\"><a href=\"";
            string endOfStartPattern = "\">";
            string endPattern = @"</a></div>";
            int previousEndIndex = 0;
            //Find the Index at \> after the URL to the Website
            for (int i = 0; i < 3; i++)
            {
                //Find Start Index of the beginning of the Start Tag
                Match beginOfStartMatch = Regex.Match(website, beginOfStartPattern);
                //Delete everything before (including) beginOfStartPattern
                int beginofStartPatternIndex = beginOfStartMatch.Index + beginOfStartPattern.Length;
                string tempWebsite = website.Substring(beginofStartPatternIndex);

                //Find Start of the Songtitle
                Match startIndexMatch = Regex.Match(tempWebsite, endOfStartPattern);
                //To get the correct Website-Index: add the number of the deleted Chars and the Length of the Tag
                int startIndex = startIndexMatch.Index + startIndexMatch.Length + beginofStartPatternIndex;

                //Find end of the Songtitle
                Match endIndexMatch = Regex.Match(tempWebsite, endPattern);
                int endIndex = endIndexMatch.Index + beginofStartPatternIndex;

                //Get the length of the Songtitle
                int lengthBetweenTags = endIndex - startIndex;

                //To get the correct Index of the complete website: add the number previous deleted Chars
                positions.Add(startIndex + previousEndIndex);
                positions.Add(lengthBetweenTags);

                //Delete the part that's already seached
                tempWebsite = website.Substring(endIndex + endPattern.Length);
                website = tempWebsite;

                //Save the number of deleted chars
                previousEndIndex += endIndex + endPattern.Length;
            }
            return positions;
        }
    }
}
