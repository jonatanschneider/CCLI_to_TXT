using System;
using System.IO;

namespace CCLI_to_TXT
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFileDirectory = "";
            if (args.Length == 1 && args[0].EndsWith(".txt"))
            {
                Logfiles.InitializingSuccededLog();
                inputFileDirectory = args[0];
            }
            else
            {
                Console.WriteLine("Bitte mit einer .TXT Datei öffnen!");
                Errorlogs.OpenedWithWrongArgumentsLog(args);
                Console.WriteLine("Zum Beenden Enter drücken.");
                Console.ReadLine();
                Environment.Exit(0);
            }

          StreamReader inputReader = new StreamReader(inputFileDirectory);//inputFileDirectory, will be replaced for start arguments
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
                if (int.TryParse(currentReaderLine, out cclinumber))
                {
                    InputIsAnInteger(cclinumber, linesInOutputFile);
                }
                else
                {
                    InputIsAString(currentReaderLine, linesInOutputFile);  
                }
            }

            inputReader.Close();
            Console.WriteLine("Enter drücken zum Beenden");
            Console.ReadLine();
        }

        private static void InputIsAnInteger(int input, string[] linesInOutputFile)
        {
            Song song = new Song(input);
            bool duplicate = song.IsSongNumberAlreadyExisting(linesInOutputFile);
            if (duplicate)
            {
                Logfiles.SongAlreadyExistsLog(song);
            }
            else
            {
                song.AddNewSongByNumber();
            }
        }

        private static void InputIsAString(string input, string[] linesInOutputFile)
        {
            Song song = new Song(input);
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
}
