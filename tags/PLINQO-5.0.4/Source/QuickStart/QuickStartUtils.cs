using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace QuickStartUtils
{
    public class QuickStartUtils
    {
        public static void ReplaceAllInDirectory(string path, string find, string replace, string[] exemptDirectories)
        {
            ReplaceAllInDirectory(path, find, replace,exemptDirectories, 0);
        }

        public static void ReplaceAllInDirectory(string path, string find, string replace)
        {
            ReplaceAllInDirectory(path, find, replace, null ,0);
        }

        private static void ReplaceAllInDirectory(string path, string find, string replace,string[] exemptDirectories, int level)
        {
            //Taken in part from http://weblogs.asp.net/israelio/archive/2004/06/23/162913.aspx
            if (level <= 3)
            {
                Console.Out.WriteLine(path);
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(path);
                foreach (string fileName in fileEntries)
                {
                    FindAndReplace(fileName, find, replace);
                }

                // Recurse into subdirectories of this directory.
                string[] subdirEntries = Directory.GetDirectories(path);
                foreach (string subdir in subdirEntries)
                {
                    // Do not iterate through reparse points
                    if ((File.GetAttributes(subdir) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    {
                        if (exemptDirectories == null || exemptDirectories.Count<string>(d => subdir.EndsWith(d)) == 0)
                            ReplaceAllInDirectory(subdir, find, replace, exemptDirectories, level + 1);
                    }
                    
                }
            }
        }
        
        public static void FindAndReplace(string fileName, string find, string replace)
        {
            //Taken From http://www.csharp411.com/searchreplace-in-files/
            
            string content;

            using (StreamReader reader = new StreamReader(fileName))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }

            content = Regex.Replace(content, find, replace);

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.Write(content);
                writer.Close();
            }
        }

        public static string FindFileInDirectory(string find, string path)
        {
            find = find.ToLower();
            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
            {
                if (fileName.ToLower().Contains(find))
                    return fileName;
            }
            return String.Empty;
        }

        public static void CopyDirectory(string pathFrom, string pathTo)
        {
            CopyDirectory(pathFrom, pathTo, 0);
        }

        private static void CopyDirectory(string pathFrom, string pathTo, int level)
        {
            if (level <= 3)
            {
                if (!Directory.Exists(pathTo))
                    Directory.CreateDirectory(pathTo);
                string[] fileEntries = Directory.GetFiles(pathFrom);
                foreach (string fileName in fileEntries)
                {
                    File.Copy(fileName, fileName.Replace(pathFrom,pathTo + @"\"));
                }

                string[] subdirEntries = Directory.GetDirectories(pathFrom);
                foreach (string subdir in subdirEntries)
                    // Do not iterate through reparse points
                    if ((File.GetAttributes(subdir) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint
                        && (File.GetAttributes(subdir) & FileAttributes.Hidden) != FileAttributes.Hidden)
                            CopyDirectory(subdir, subdir.Replace(pathFrom,pathTo + @"\"), level + 1);
            }
        }

        public static void ReplaceAllFilesInDirectory(string directoryFind, string directoryReplace)
        {
            ReplaceAllFilesInDirectory(directoryFind, directoryReplace, String.Empty);
        }

        public static void ReplaceAllFilesInDirectory(string directoryFind, string directoryReplace, string fileExtension)
        {
            if(fileExtension != String.Empty)
            {
                foreach(string file in Directory.GetFiles(directoryFind,fileExtension))
                {
                    File.Copy(file.Replace(directoryFind, directoryReplace), file, false);
                }
            }
            else
            {
                foreach(string file in Directory.GetFiles(directoryFind))
                {
                    File.Copy(file.Replace(directoryFind, directoryReplace), file, false);
                }
            }
        }

        public static string EnsureMultipleResultSets(string connectionString)
        {
            if (connectionString.Contains("MultipleActiveResultSets"))
                return connectionString;

            string s = connectionString.Trim();
            return s + (s.EndsWith(";") ? "MultipleActiveResultSets=true;" : ";MultipleActiveResultSets=true;");
        }
    }
}
