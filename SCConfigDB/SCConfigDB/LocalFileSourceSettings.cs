using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Defter.StarCitizen.ConfigDB
{
    public class LocalFileSourceSettings : IFileSourceSettings
    {
        public string DatabasePath { get; set; }
        public string DatabaseFilePath => DatabasePaths.DatabaseFilePath(DatabasePath);
        public string DatabaseTranslateFilePath(string language) => DatabasePaths.DatabaseTranslateFilePath(DatabasePath, language);
        public string JsonSchemaFilePath => DatabasePaths.JsonSchemaFilePath(DatabasePath);
        public string JsonTranslateSchemaFilePath => DatabasePaths.JsonTranslateSchemaFilePath(DatabasePath);

        public LocalFileSourceSettings(string path)
        {
            DatabasePath = path;
        }

        public ISet<string> GetDatabaseLanguages()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(DatabaseFilePath);
            var subDirectoriesNames = directoryInfo.EnumerateDirectories().Select(d => d.Name);
            return new HashSet<string>(subDirectoriesNames, StringComparer.OrdinalIgnoreCase);
        }
    }
}
