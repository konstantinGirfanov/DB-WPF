namespace DummyDatabase.Core
{
    public static class WorkWithFiles
    {
        public static List<string> GetFolderFiles(string folderName)
        {
            var files = Directory.GetFiles(GetFolderPath(folderName));

            List<string> stringFiles = new();
            foreach (var file in files)
            {
                stringFiles.Add(file.Split("\\")[^1]);
            }

            return stringFiles;
        }

        public static string GetFolderPath(string folderName)
        {
            return $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\{folderName}";
        }

        public static string GetFilePath(string folderName, string fileName)
        {
            if(folderName == "schemes")
            {
                return $"{GetFolderPath(folderName)}\\{fileName}.json";
            }
            else
            {
                return $"{GetFolderPath(folderName)}\\{fileName}";
            }
        }

        public static string GetSchemeDataName(string schemeName)
        {
            return $"{schemeName.Split('.')[0]}Data.txt";
        }
    }
}