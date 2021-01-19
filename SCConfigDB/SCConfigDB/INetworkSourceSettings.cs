namespace Defter.StarCitizen.ConfigDB
{
    public interface INetworkSourceSettings
    {
        abstract string DatabaseUrl { get; }
        abstract string DatabaseTranslateUrl(string language);
        abstract string? JsonSchemaUrl { get; }
        abstract string? JsonTranslateSchemaUrl { get; }
    }
}
