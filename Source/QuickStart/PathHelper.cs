using System;
using System.IO;

namespace QuickStart
{
    public class PathHelper
    {
        public PathHelper(string fileName, string directoryName, string parentPath)
        {
            FileName = fileName;
            DirectoryName = directoryName;

            DirectoryFile = Path.Combine(DirectoryName, FileName);

            DirectoryPath = Path.Combine(parentPath, DirectoryName);
            FilePath = Path.Combine(DirectoryPath, FileName);
        }

        public string FileName { get; private set; }
        public string DirectoryName { get; private set; }

        public string DirectoryFile { get; private set; }

        public string DirectoryPath { get; private set; }
        public string FilePath { get; private set; }
    }

}
