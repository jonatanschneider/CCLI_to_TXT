using System;
using System.Collections.Generic;
using System.IO;

namespace CCLI_to_TXT
{
    class OutputHandling
    {
        //absoulte path for output file missing
        /// <summary>
        /// Writes Songinfos to Output File
        /// </summary>
        /// <param name="songname">Songname</param>
        /// <param name="songNumber">CCLI-Number</param>
        public static void NewSong(string songname, int songNumber)
        {
            StreamWriter output = new StreamWriter("Output.ccli", true);
            output.Write(songNumber);
            output.Write(" - ");
            output.Write(songname);
            output.WriteLine();
            output.Close();
            Logfiles.SongAddedLog(songname);
        }

        /// <summary>
        /// Creates File if necessary
        /// </summary>
        /// <param name="filename">Filename</param>
        public static void CreateFile(string filename)
        {
            StreamWriter writer = File.CreateText(filename);
            writer.Close();
            Logfiles.FileCreatedLog(filename);
        }
       
    }
}
