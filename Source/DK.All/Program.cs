using System;
using System.Collections.Generic;
using System.IO;

namespace DK.All
{
    ///<summary>
    ///</summary>
    public class Program
    {
        private static IDictionary<String, String> projects;
        private static String chosenProject;
        private static IDictionary<String, String> currentLibs;

        ///<summary>
        ///</summary>
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
            Console.WriteLine("KEON LIBRARY");
            Console.WriteLine("===================================");
        }



        private static void setProjectOptions()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var mainDir = new DirectoryInfo(projectDir)
				.Parent? // bin
	            .Parent? // DK.All
	            .Parent? // Source
	            .Parent? // DK
	            .Parent? // mother of all
	            .FullName;

	        if (mainDir == null)
		        throw new Exception("Path not found!!!");

            projects = new Dictionary<String, String>
            {
                {"DFM", Path.Combine(mainDir, "DfM", "site", "Library") },
                {"Stories", Path.Combine(mainDir, "MEAK", "Site", "Site", "Library") }
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
                   !projects.Keys.Contains(chosenProject)
                   && chosenProject != "All";
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

                if (fileName.Contains("DK.All")) continue;

                currentLibs.Add(fileName, lib);
            }
        }

        private static IEnumerable<String> getLibs()
        {
            var currentDirectoryPath = Directory.GetCurrentDirectory();

            return Directory.GetFiles(currentDirectoryPath, "DK.*");
        }



        private static void copyLibs()
        {
            if (chosenProject != "All")
                copyLibs(chosenProject, projects[chosenProject]);
            else
                foreach (var project in projects)
                {
                    copyLibs(project.Key, project.Value);
                }

            Console.WriteLine("Done.");
        }

        private static void copyLibs(String name, String path)
        {
            Console.WriteLine($"Copying to ${name}");

            foreach (var lib in currentLibs)
            {
                var fileName = lib.Key;
                var original = lib.Value;

                var libCopy = Path.Combine(path, "DK", fileName);

                File.Copy(original, libCopy, true);
                Console.WriteLine($"Copied: {libCopy}");
            }

            Console.WriteLine();
        }
    }
}
