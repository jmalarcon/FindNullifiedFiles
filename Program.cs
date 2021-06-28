using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindNullifiedFiles
{
    class Program
    {
        private static int numFolders = 0;
        private static int numFiles = 0;

        static void Main(string[] args)
        {
            if (args.Length == 0 || (args[0] == "-?") || (args[0] == "/?"))
            {
                ShowHelp();
                return;
            }

            string path = Path.GetFullPath(args[0]);
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Folder \"{0}\" does not exist. Exiting...", path);
                return;
            }

            int numDaysAgo = 30;
            int numNulls = 10;
            if (args.Length > 1)
            {
                _ = int.TryParse(args[1], out numDaysAgo);
                if (numDaysAgo == 0) numDaysAgo = 30;
            }
            if (args.Length > 2)
            {
                _ = int.TryParse(args[2], out numNulls);
                if (numNulls == 0) numNulls = 10;
            }

            Console.WriteLine("Finding nullified files...");
            Stopwatch sw = Stopwatch.StartNew();
            Processfolder(path, DateTime.Now.AddDays(-numDaysAgo), numNulls);
            sw.Stop();
            Console.WriteLine("{0}: {1} nullified files found in {2} folders in {3}",
                              DateTime.Now,
                              numFiles,
                              numFolders,
                              sw.Elapsed);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Finds all the files in a folder that have at least the first 10 bytes as null and have been modified 30 days ago or less.");
            Console.WriteLine("Usage: FindNullifiedFiles folder [max number of days the files have been modified(30)] [num of nulls at the beginning of file (10)]");
            Console.WriteLine("Ej: FindNullifiedFiles C:\\MyFolder");
            Console.WriteLine("Ej: FindNullifiedFiles C:\\MyFolder 15 30");
            Console.WriteLine("(c) José M. Alarcón [www.jasoft.org]");
        }

        private static void Processfolder(string path, DateTime fromDate, int numNulls)
        {
            numFolders++;
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles().Where(file => file.LastWriteTime >= fromDate).ToArray<FileInfo>();
            foreach (FileInfo file in files)
            {
                try
                {
                    if (IsNullifiedFile(file.FullName, numNulls, file.Length))
                    {
                        numFiles++;
                        Console.WriteLine(file.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading file {0}: {1}", file.FullName, ex.Message);
                }
            }

            //Ahora recorremos los subdirectorios pertinentes
            DirectoryInfo[] folders = di.GetDirectories();

            foreach (DirectoryInfo carpeta in folders)
            {
                Processfolder(carpeta.FullName, fromDate, numNulls);
            }

        }

        private static bool IsNullifiedFile(string filePath, int minNumNullsToFind, long fileLength)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader sr = new BinaryReader(fs);
            //Check if it's made of nulls
            int numOfNullsRead = 0;
            while ( numOfNullsRead < minNumNullsToFind && numOfNullsRead < fileLength && sr.ReadByte() == (byte)0)
            {
                numOfNullsRead++;
            }
            sr.Close();
            //If all nulls, is a nullified file
            return numOfNullsRead == minNumNullsToFind || numOfNullsRead == fileLength;
        }
    }
}
