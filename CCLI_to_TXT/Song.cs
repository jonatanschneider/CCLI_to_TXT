using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace CCLI_to_TXT
{
    class Song
    {
        public Song(string name)
        {
            this.name = name;
        }
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
        public string MakeNameWebSearchable()
        {
            string name = this.name.Replace(" ", "+");
            return name;
        }

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
        public void AddNewSongByNumber()
        {
            this.GetSongnameByNumber();
            if (this.name != "Error")
            {
                OutputHandling.NewSong(this.name, this.number);
            }
        }

        public void GetSongnameByNumber()
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
                Errorlogs.SongNameNotFoundLog(e, this.number);
                string noSongname = "noSongnameError";
                return noSongname;
            }
        }

        public int[] GetSongnamePositionInWebsiteByNumber(string website)
        {
            //Between these two tags is the Songname
            string startTag = "<title>";
            string endTag = "</title>";
            int[] startTagAndLength = new int[2];

            //Get Position of the songname
            int indexOfStartTag = website.IndexOf(startTag) + startTag.Length;
            int indexOfEndTag = website.IndexOf(endTag);
            int lengthBetweenTags = indexOfEndTag - indexOfStartTag;

            //Write Values in Output Array
            startTagAndLength[0] = indexOfStartTag;
            startTagAndLength[1] = lengthBetweenTags;

            return startTagAndLength;
        }

        //Method with name as input
        public void AddNewSongByName()
        {
            if (this.name != "Error")
            {
                OutputHandling.NewSong(this.name, this.number);
            }
        }

        public void ChooseCorrectSong()
        {
            List<string> songs = this.GetSongnames();
            for (int i = 0; i < 3; i++)
            {
                Console.Write((i + 1) + " - ");
                Console.Write(songs[i]);
                Console.WriteLine();
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
                Errorlogs.InputNoCorrectNumber(e);
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

        public List<string> GetSongnames()
        {
            List<string> songnames = new List<string>(2);

            string website = this.DownloadWebsiteByName();
            List<int> positions = this.GetSongnamePositionsInWebsite(website);
            this.GetNumberBySongname(website);

            for (int i = 0; i < 3; i++)
            {
                string name = WebUtility.HtmlDecode(website.Substring(positions[0], positions[1]));
                songnames.Add(name);

                //Remove unncecessary old values
                int item1 = positions[0];
                int item2 = positions[1];
                positions.Remove(item1);
                positions.Remove(item2);
            }

            return songnames;
        }

        public void GetNumberBySongname(string website)
        {
            string startTag = "<div class=\"songTitle\"><a href=\"/songs/";
            string endTag = WebUtility.HtmlEncode(this.name)+ "</a></div>";

            //Get Position of the songname
            int indexOfStartTag = website.IndexOf(startTag) + startTag.Length;
            string tempWebsite = website.Substring(indexOfStartTag);
            int indexOfEndTag = tempWebsite.IndexOf(endTag) +indexOfStartTag;

            int ccliNumberStandardLength = 7;
            this.number = Convert.ToInt32(website.Substring(indexOfStartTag, ccliNumberStandardLength));

        }

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
        //Clean up!
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
                //To get the correct Website-Index added the number of the deleted Chars and the Length of the Tag
                int startIndex = startIndexMatch.Index + startIndexMatch.Length + beginofStartPatternIndex;

                //Find end of the Songtitle
                Match endIndexMatch = Regex.Match(tempWebsite, endPattern);
                int endIndex = endIndexMatch.Index + beginofStartPatternIndex;

                //Get the length of the Songtitle
                int lengthBetweenTags = endIndex - startIndex;

                positions.Add(startIndex + previousEndIndex);
                positions.Add(lengthBetweenTags);

                //Delete the part that's already seached
                tempWebsite = website.Substring(endIndex + endPattern.Length);
                website = tempWebsite;

                previousEndIndex += endIndex + endPattern.Length;
            }
            return positions;
        }

        
    }
}
