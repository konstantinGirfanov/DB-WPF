using System.IO;

namespace DBCore
{
    static class WorkWithFiles
    {
        public static string GetSchemePath(string schemeName)
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\" + "schemas" + "\\" + schemeName;
        }

        public static string GetDataPath(string schemeName)
        {
            string schemeDataName = GetSchemeDataName(schemeName);
            return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\" + "data" + "\\" + schemeDataName;
        }

        private static string GetSchemeDataName(string schemeName)
        {
            return schemeName.Split('.')[0] + "Data.txt";
        }
    }
}