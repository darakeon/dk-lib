using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ak.DataAccess.XML;

namespace Ak.All
{
    ///<summary>
    ///</summary>
    public class Program
    {
        private static IDictionary<String, String> projects;
        private static String chosenProject;
        private static IDictionary<String, String> currentLibs;

        public static void Main()
        {
            welcome();

            setProjectOptions();

            setPath();

            setLibs();

            copyLibs();

            Console.Read();
        }



        private static void welcome()
        {
            Console.WriteLine("AKEON LIBRARY");
            Console.WriteLine("===================================");
        }



        private static void setProjectOptions()
        {
            projects = new Dictionary<String, String>
                           {
                               {"DFM", @"D:\Lucas\Akeon\03 - DFM\Source\Library"},
                               {"Responde", @"D:\Lucas\Akeon\02 - Responde\Source\Library"},
                               {"Stories", @"D:\Lucas\Akeon\06 - Stories\Source\Library"}
                           };
        }



        private static void setPath()
        {
            while (wrongChoice())
            {
                showProjectOptions();

                chosenProject = Console.ReadLine();
                Console.WriteLine();
            }
        }

        private static bool wrongChoice()
        {
            return chosenProject == null || 
                   (!projects.Keys.Contains(chosenProject)
                        && chosenProject != "All");
        }

        private static void showProjectOptions()
        {
            Console.Write("Set your project library path (");
            Console.Write("All");

            foreach (var project in projects)
            {
                Console.Write(@" / {0}", project.Key);
            }

            Console.WriteLine("): ");
        }



        private static void setLibs()
        {
            currentLibs = new Dictionary<String, String>();

            foreach (var lib in getLibs())
            {
                var fileName = new FileInfo(lib).Name;

                if (fileName.Contains("Ak.All")) continue;

                currentLibs.Add(fileName, lib);
            }
        }

        private static IEnumerable<String> getLibs()
        {
            var currentDirectoryPath = Directory.GetCurrentDirectory();

            return Directory.GetFiles(currentDirectoryPath, "Ak.*");
        }



        private static void copyLibs()
        {
            if (chosenProject != "All")
                copyLibs(projects[chosenProject]);
            else
                foreach (var project in projects)
                {
                    copyLibs(project.Value);
                }
        }

        private static void copyLibs(string path)
        {
            foreach (var lib in currentLibs)
            {
                var fileName = lib.Key;
                var original = lib.Value;

                var libCopy = Path.Combine(path, "Ak", fileName);

                File.Copy(original, libCopy, true);
                Console.WriteLine("Copied: {0}", libCopy);
            }

            Console.WriteLine();
        }
    }
}
