using System;
using System.IO;

namespace CCLI_to_TXT
{
    class Program
    {
        static void Main(string[] args)
        {

            StreamReader inputReader = new StreamReader("CCLI.txt");//inputFileDirectory, will be replaced for start arguments
            string currentReaderLine;
            string[] linesInOutputFile = new string[0];
            
            try
            {
                //Reads every line in the Output File to check for duplicates later
                linesInOutputFile = File.ReadAllLines("Output.ccli");
            }
            catch (FileNotFoundException e)
            {
                //Creates File Output.ccli if necessary
                Errorlogs.FileNotFoundLog(e);
                OutputHandling.CreateFile(e.FileName);
            }

            //Reads Input File
            while ((currentReaderLine = inputReader.ReadLine()) != null)
            {
                int cclinumber;
                //Input is a Number
                if (int.TryParse(currentReaderLine, out cclinumber))
                {
                    Song song = new Song(cclinumber);

                    if (song.IsSongNumberAlreadyExisting(linesInOutputFile))
                    {
                        Logfiles.SongAlreadyExistsLog(song);
                    }
                    else
                    {
                        song.AddNewSongByNumber();
                    }
                }
                //Input is a String
                else
                {
                    Song song = new Song(currentReaderLine);
                    song.ChooseCorrectSong();
                    bool duplicate = song.IsSongNumberAlreadyExisting(linesInOutputFile);
                    if (duplicate)
                    {
                        Logfiles.SongAlreadyExistsLog(song);
                    }
                    else
                    {
                        song.AddNewSongByName();
                    }
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
