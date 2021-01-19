using System.Collections.Generic;

namespace Defter.StarCitizen.ConfigDB
{
    public interface IFileSourceSettings
    {
        string DatabasePath { get; set; }
        string DatabaseFilePath { get; }
        string DatabaseTranslateFilePath(string language);
        string? JsonSchemaFilePath { get; }
        string? JsonTranslateSchemaFilePath { get; }
        ISet<string> GetDatabaseLanguages();
    }
}
