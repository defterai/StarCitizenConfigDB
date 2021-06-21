namespace Defter.StarCitizen.ConfigDB
{
    public interface INetworkSourceSettings
    {
        string RepositoryUrl { get; }
        string DatabaseUrl { get; }
        string DatabaseTranslateUrl(string language);
        string? JsonSchemaUrl { get; }
        string? JsonTranslateSchemaUrl { get; }
    }
}
