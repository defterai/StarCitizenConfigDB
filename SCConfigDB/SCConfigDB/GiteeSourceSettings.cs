namespace Defter.StarCitizen.ConfigDB
{
    public class GiteeSourceSettings : INetworkSourceSettings
    {
        public const string DefaultUser = "defter";
        public const string DefaultRepository = "StarCitizenConfigDB";
        public const string DefaultBranch = "master";

        public string User { get; set; } = DefaultUser;
        public string Repository { get; set; } = DefaultRepository;
        public string Branch { get; set; } = DefaultBranch;
        public string RepositoryUrl => @$"https://gitee.com/{User}/{Repository}";
        public string SourcesUrl => @$"https://gitee.com/{User}/{Repository}/raw/{Branch}";
        public string DatabaseUrl => DatabasePaths.DatabaseFilePath(SourcesUrl);
        public string DatabaseTranslateUrl(string language) => DatabasePaths.DatabaseTranslateFilePath(SourcesUrl, language);
        public string JsonSchemaUrl => DatabasePaths.JsonSchemaFilePath(SourcesUrl);
        public string JsonTranslateSchemaUrl => DatabasePaths.JsonTranslateSchemaFilePath(SourcesUrl);
    }
}
