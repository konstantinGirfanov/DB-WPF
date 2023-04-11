using System.Collections.Generic;
using System.IO;

namespace DBCore
{
    static class WorkWithFiles
    {
        public static List<string> GetFolderFiles(string folderName)
        {
            string folderPath = GetFolderPath(folderName);
            var files = Directory.GetFiles(folderPath);

            List<string> stringFiles = new();
            foreach (var file in files)
            {
                stringFiles.Add(file.Split("\\")[^1]);
            }

            return stringFiles;
        }

        public static string GetFolderPath(string folderName)
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + $"\\{folderName}";
        }

        private static string GetSchemeDataName(string schemeName)
        {
            return schemeName.Split('.')[0] + "Data.txt";
        }
    }
}