﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Ak.DataAccess.NHibernate.Helpers;

namespace Ak.All
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
            Console.WriteLine("AKEON LIBRARY");
            Console.WriteLine("===================================");
        }



        private static void setProjectOptions()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var localDir = projectDir.IndexOf("Akeon");
            var mainDir = projectDir.Substring(0, localDir);

            projects = new Dictionary<String, String>
                           {
                               {"DFM", mainDir + @"Akeon\03 - DFM\Source\Library"},
                               {"Responde", mainDir + @"Akeon\02 - Responde\Source\Library"},
                               {"Stories", mainDir + @"Akeon\06 - Stories\Source\Library"}
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
            Console.WriteLine("Copying to {0}", name);

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
