namespace Defter.StarCitizen.ConfigDB
{
    public static class LocalSourceSettings
    {
        public static string DatabaseFilePath(string path) => $"{path}/database/config.json";
        public static string DatabaseTranslateFilePath(string path, string language) => $"{path}/database/{language}/config.json";
    }
}
