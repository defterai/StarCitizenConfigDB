namespace Defter.StarCitizen.ConfigDB
{
    public static class DatabasePaths
    {
        public static string DatabasePath(string path) => $"{path}/database";
        public static string DatabaseFilePath(string path) => $"{path}/database/config.json";
        public static string DatabaseTranslateFilePath(string path, string language) => $"{path}/database/{language}/config.json";
        public static string JsonSchemaFilePath(string path) => $"{path}/schema.json";
        public static string JsonTranslateSchemaFilePath(string path) => $"{path}/schema_translate.json";
    }
}
