using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CCLI_to_TXT
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader inputReader = new StreamReader("CCLI.txt");//inputFileDirectory
            string currentReaderLine;
            string[] linesInOutputFile = new string[0];

            //Creates File Output.ccli if necessary
            try {
                //Reads every line in the Output File to check for duplicates later
                 linesInOutputFile = File.ReadAllLines("Output.ccli");
            }
            catch (FileNotFoundException e)
            {
                Errorlogs.FileNotFoundLog(e);
                OutputHandling.CreateFile(e.FileName);
            }
            //Reads Input File
            while ((currentReaderLine = inputReader.ReadLine())!= null)
            {
                int ccliNumber = Convert.ToInt32(currentReaderLine);
                Song song = new Song(ccliNumber);
                //Check for duplicates
                if (song.IsSongNumberAlreadyExisting(linesInOutputFile))
                {
                    Logfiles.SongAlreadyExistsLog(song);
                }
                else
                {
                    song.AddNewSongByNumber();
                }
                
            }
            inputReader.Close();
            Console.WriteLine("...");
            Console.ReadLine();
        }
    }
}

/* Aufruf des Programms mit .txt Datei
string inputFileDirectory = "";
            if (args.Length == 1 && args[0].EndsWith(".txt"))
            {
                Logfiles.InitializingSuccededLog();
                inputFileDirectory = args[0];
                //For Debugging Puropses:
                //inputFileDirectory = "CCLI.txt";
            }
            else
            {
                Console.WriteLine("Bitte mit einer .TXT Datei öffnen!");
                Errorlogs.OpenedWithWrongArgumentsLog(args);
                Console.WriteLine("Zum Beenden Enter drücken.");
                Console.ReadLine();
                Environment.Exit(0);
            } */
