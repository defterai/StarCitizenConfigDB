using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Defter.StarCitizen.ConfigDB
{
    public static class LocalSourceSettings
    {
        public static string DatabaseFilePath(string path) => $"{path}/database/config.json";
        public static string DatabaseTranslateFilePath(string path, string language) => $"{path}/database/{language}/config.json";

        public static HashSet<string> GetDatabaseLanguages(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{path}/database");
            var subDirectoriesNames = directoryInfo.EnumerateDirectories().Select(d => d.Name);
            return new HashSet<string>(subDirectoriesNames, StringComparer.OrdinalIgnoreCase);
        }
    }
}
