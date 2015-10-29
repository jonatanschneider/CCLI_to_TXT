using System.Net;
using System;

namespace CCLI_to_TXT
{
    class Song
    {
        //Constructors
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

        public void GetSongname()
        {
            string website = DownloadWebsite();
            if (website != "noSongnameError")
            {
                int[] positions = GetSongnamePositionInWebsite(website);
                //Decode HTML-Text to Plain-Text
                this.name = WebUtility.HtmlDecode(website.Substring(positions[0], positions[1]));
            }
            else
            {
                this.name = "Error";
            }
        }

        public int[] GetSongnamePositionInWebsite(string website)
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

        public string DownloadWebsite()
        {
            WebClient webclient = new System.Net.WebClient();
            try
            {
                string webdata = webclient.DownloadString("https://de.songselect.com/songs/" + this.number);
                return webdata;
            }
            //Abfangen von 404-Fehler
            catch (System.Net.WebException e)
            {
                Console.WriteLine("Fehler bei Song {0}. Bitte Error-Logs überprüfen!", this.number);
                Errorlogs.SongNameNotFoundLog(e, this.number);
                string noSongname = "noSongnameError";
                return noSongname;
            }
        }

        public void AddNewSongByNumber()
        {
            this.GetSongname();
            if (this.name != "Error")
            {
                OutputHandling.NewSong(this.name, this.number);
            }
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
    }
}
