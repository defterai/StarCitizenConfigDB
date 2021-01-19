using System.IO;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class DbJsonTextWriter : JsonTextWriter
    {
        public DbJsonTextWriter(TextWriter writer) : base(writer)
        {
            Formatting = Formatting.Indented;
            IndentChar = '\t';
            Indentation = 1;
            QuoteChar = '"';
            QuoteName = true;
        }
    }
}
