using System;
using System.Collections.Generic;
using System.IO;

namespace CCLI_to_TXT
{
    class Logfiles
    {
        public static void PrintLogs(List<string> log)
        {
            StreamWriter output = new StreamWriter("logfile.logdb", true);

            output.WriteLine(DateTime.Now +" - " +Environment.UserDomainName +"\\" +Environment.UserName);
            foreach (string item in log)
            {
                output.WriteLine(item);
            }

            output.WriteLine("-------------");

            output.Close();
        }

        public static void InitializingSuccededLog()
        {
            List<string> log = new List<string>();
            log.Add("Initialisierung mit korrekter Datei erfolgt");
            Logfiles.PrintLogs(log);
        }

        public static void SongAlreadyExistsLog(Song song)
        {
            List<string> log = new List<string>();
            log.Add("SongAlreadyExistsLog");
            log.Add("Eintrag \"" + song.Number + "\" schon vorhanden");
            Logfiles.PrintLogs(log);
        }

        public static void SongAlreadyExistsLog(string cclinumber)
        {
            List<string> log = new List<string>();
            log.Add("SongAlreadyExistsLog");
            log.Add("Eintrag \"" + cclinumber + "\" schon vorhanden");
            Logfiles.PrintLogs(log);
        }

        public static void SongAddedLog(Song song)
        {
            List<string> log = new List<string>();
            log.Add("SongAddedLog");
            log.Add("Song \"" + song.Name + "\" hinzugefügt");
            Logfiles.PrintLogs(log);
        }

        public static void SongAddedLog(string songname)
        {
            List<string> log = new List<string>();
            log.Add("SongAddedLog");
            log.Add("Song \"" + songname + "\" hinzugefügt");
            Logfiles.PrintLogs(log);
        }

       /* public static void SkippedSongLog(Song song)
        {
            List<string> log = new List<string>();
            log.Add("SongSkippedLog");
            log.Add("Song \"" + song.Name + "\" wurde übersprungen");
            Logfiles.PrintLogs(log);
        }

        public static void SkippedSongLog(string songname)
        {
            List<string> log = new List<string>();
            log.Add("SongSkippedLog");
            log.Add("Song \"" + songname + "\" wurde übersprungen");
            Logfiles.PrintLogs(log);
        }

        public static void SkippedSongLog(int cclinumber)
        {
            List<string> log = new List<string>();
            log.Add("SongSkippedLog");
            log.Add("Song \"" + cclinumber + "\" wurde übersprungen");
            Logfiles.PrintLogs(log);
        }*/

        public static void FileCreatedLog(string filename)
        {
            List<string> log = new List<string>();
            log.Add("FileCreatedLog");
            log.Add("Datei \"" + filename + "\" erstellt");
            Logfiles.PrintLogs(log);
        }
        
    }
}
