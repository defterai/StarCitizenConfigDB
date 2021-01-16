using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class SettingFactory
    {
        private Dictionary<ValueJsonType, BaseSetting.IFactory> Factories { get; } = new Dictionary<ValueJsonType, BaseSetting.IFactory>();

        public SettingFactory()
        {
            Register(ValueJsonType.Bool, new BooleanSetting.Factory());
            Register(ValueJsonType.Int, new IntegerSetting.Factory());
            Register(ValueJsonType.Float, new FloatSetting.Factory());
            Register(ValueJsonType.RangeInt, new IntegerSetting.Factory());
            Register(ValueJsonType.RangeFloat, new FloatSetting.Factory());
        }

        public void Register(ValueJsonType type, BaseSetting.IFactory factory) => Factories.Add(type, factory);

        public BaseSetting Build(SettingJsonNode node) => Factories[node.Values.Type].Build(node);
    }
}
