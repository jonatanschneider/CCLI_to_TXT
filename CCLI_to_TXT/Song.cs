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

    }
}
