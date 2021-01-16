namespace Defter.StarCitizen.ConfigDB
{
    public sealed class GitSourceSettings
    {
        public const string DefaultUser = "defterai";
        public const string DefaultRepository = "StarCitizenConfigDB";
        public const string DefaultBranch = "master";

        public string User { get; set; } = DefaultUser;
        public string Repository { get; set; } = DefaultRepository;
        public string Branch { get; set; } = DefaultBranch;
        public string RepositoryUrl => @$"https://github.com/{User}/{Repository}";
        public string DatabaseUrlPrefix => @$"https://raw.githubusercontent.com/{User}/{Repository}/{Branch}";
        public string DatabaseUrl => LocalSourceSettings.DatabaseFilePath(DatabaseUrlPrefix);
        public string DatabaseTranslateUrl(string language) => LocalSourceSettings.DatabaseTranslateFilePath(DatabaseUrlPrefix, language);
    }
}
