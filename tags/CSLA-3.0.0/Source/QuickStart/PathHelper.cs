using System;
using System.Collections.Generic;
using System.IO;

namespace QuickStart
{
    public static class PathHelper
    {
        public static IEnumerable<string> GetFiles(string directory, SearchOption searchOption, params string[] searchPatterns)
        {
            var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pattern in searchPatterns)
                files.UnionWith(Directory.GetFiles(directory, pattern, SearchOption.AllDirectories));

            return files;
        }
    }

}
