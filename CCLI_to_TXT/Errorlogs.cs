using System;
using System.Collections.Generic;
using System.IO;

namespace CCLI_to_TXT
{
    class Errorlogs
    {
        /// <summary>
        /// Print out the log with DateTime.Now and Windows User Domain + Name
        /// </summary>
        /// <param name="log">Log to print</param>
        public static void PrintLogs(List<string> log)
        {
            StreamWriter output = new StreamWriter("errorlogs.logdb", true);

            output.WriteLine(DateTime.Now + " - " + Environment.UserDomainName + "\\" + Environment.UserName);
            foreach (string item in log)
            {
                output.WriteLine(item);
            }
            output.WriteLine("-------------");

            output.Close();
        }

        public static void OpenedWithWrongArgumentsLog(string[] args)
        {
            List<string> log = new List<string>();
            log.Add("OpenedWithWrongArgumentsError");
            log.Add("Folgende Argumente wurden angegeben:");
            foreach (string item in args)
            {
                log.Add(item);
            }
            Errorlogs.PrintLogs(log);
        }

        public static void SongNameNotFoundLog(System.Net.WebException e, Song song)
        {
            List<string> log = new List<string>();
            log.Add("SongNameNotFoundError");
            log.Add("Fehlermeldung: "+e.Message);
            log.Add("Der Song \"" + song.Number + "\" konnte nicht in der CCLI-Datenbank gefunden werden.");
            log.Add("Fehlerbehebung: CCLI-Nummer überprüfen, ggf. ändern und Programm neustarten");
            Errorlogs.PrintLogs(log);
        }

        public static void SongnameNotFoundLog(System.Net.WebException e, int ccliNumber)
        {
            List<string> log = new List<string>();
            log.Add("SongnameNotFoundError");
            log.Add("Fehlermeldung: " + e.Message);
            log.Add("Der Song \"" + ccliNumber + "\" konnte nicht in der CCLI-Datenbank gefunden werden.");
            log.Add("Fehlerbehebung: CCLI-Nummer überprüfen, ggf. ändern und Programm neustarten");
            Errorlogs.PrintLogs(log);
        }

        public static void SongNameNotFoundLog(System.Net.WebException e, string songname)
        {
            List<string> log = new List<string>();
            log.Add("SongNameNotFoundError");
            log.Add("Fehlermeldung: " + e.Message);
            log.Add("Der Song \"" + songname + "\" konnte nicht in der CCLI-Datenbank gefunden werden.");
            log.Add("Fehlerbehebung: CCLI-Nummer überprüfen, ggf. ändern und Programm neustarten");
            Errorlogs.PrintLogs(log);
        }

        public static void FileNotFoundLog(FileNotFoundException e)
        {
            List<string> log = new List<string>();
            log.Add("FileNotFoundError");
            log.Add(e.Message);
            Errorlogs.PrintLogs(log);
        }

        public static void InputNotANumberLog(FormatException e)
        {
            List<string> log = new List<string>();
            log.Add("InputNotANumberError");
            log.Add("Fehlermeldung: " + e.Message);
            log.Add("Fehler bei der Auswahl des Songs.");
            log.Add("Fehlerbehebung: Programm mit Song erneut starten, nur 1, 2 oder 3 eingeben.");
            Errorlogs.PrintLogs(log);
        }

        public static void InputNoCorrectNumberLog(ArgumentOutOfRangeException e)
        {
            List<string> log = new List<string>();
            log.Add("InputNoCorrectNumberError");
            log.Add("Fehlermeldung: " +e.Message);
            log.Add("Fehler bei der Auswahl des Songs.");
            log.Add("Fehlerbehebung: Programm mit Song erneut starten, nur 1, 2 oder 3 eingeben.");
            Errorlogs.PrintLogs(log);
        }
    }
}
