namespace Defter.StarCitizen.ConfigDB
{
    public class GitHubSourceSettings : INetworkSourceSettings
    {
        public const string DefaultUser = "defterai";
        public const string DefaultRepository = "StarCitizenConfigDB";
        public const string DefaultBranch = "master";

        public string User { get; set; } = DefaultUser;
        public string Repository { get; set; } = DefaultRepository;
        public string Branch { get; set; } = DefaultBranch;
        public string RepositoryUrl => @$"https://github.com/{User}/{Repository}";
        public string SourcesUrl => @$"https://raw.githubusercontent.com/{User}/{Repository}/{Branch}";
        public string DatabaseUrl => DatabasePaths.DatabaseFilePath(SourcesUrl);
        public string DatabaseTranslateUrl(string language) => DatabasePaths.DatabaseTranslateFilePath(SourcesUrl, language);
        public string JsonSchemaUrl => DatabasePaths.JsonSchemaFilePath(SourcesUrl);
        public string JsonTranslateSchemaUrl => DatabasePaths.JsonTranslateSchemaFilePath(SourcesUrl);
    }
}
