namespace Defter.StarCitizen.ConfigDB
{
    public interface INetworkSourceSettings
    {
        string DatabaseUrl { get; }
        string DatabaseTranslateUrl(string language);
        string? JsonSchemaUrl { get; }
        string? JsonTranslateSchemaUrl { get; }
    }
}
