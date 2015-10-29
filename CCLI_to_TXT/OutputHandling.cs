using System;
using System.Collections.Generic;
using System.IO;

namespace CCLI_to_TXT
{
    class OutputHandling
    {
        public static void NewSong(string songname, int songNumber)//absoulte path for output file missing
        {
            StreamWriter output = new StreamWriter("Output.ccli", true);
            output.Write(songNumber);
            output.Write(" - ");
            output.Write(songname);
            output.WriteLine();
            output.Close();
            Logfiles.SongAddedLog(songname);
        }

        public static void CreateFile(string filename)
        {
            StreamWriter writer = File.CreateText(filename);
            writer.Close();
            Logfiles.FileCreatedLog(filename);
        }
       
    }
}
