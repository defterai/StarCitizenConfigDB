using System;
using System.Net;
using System.Net.Http;
using Defter.StarCitizen.ConfigDB;
using Defter.StarCitizen.ConfigDB.Model;

namespace Defter.StarCitizen.TestApplication
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();

            ConfigDataLoader? configDataLoader = null;
            do
            {
                Console.Write("Press to choose load data source [N - network / L - local]? ");
                var pressKey = Console.ReadKey();
                Console.WriteLine("");
                if (pressKey.KeyChar == 'n' || pressKey.KeyChar == 'N')
                    configDataLoader = new NetworkConfigDataLoader(client, new GitSourceSettings());
                else if (pressKey.KeyChar == 'l' || pressKey.KeyChar == 'L')
                    configDataLoader = new LocalConfigDataLoader(@"E:\Work\C-sharp\StarCitizenConfigDb");
            } while (configDataLoader == null);

            try
            {
                configDataLoader.LoadDatabase();
                configDataLoader.LoadTranslation("ua");
                PrintConfigData(configDataLoader.BuildData("ua"));
            } catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private static void PrintConfigData(ConfigData data)
        {
            if (data.CommandCategories.Count != 0)
            {
                Console.WriteLine("Command categories:");
                foreach (var commandCategory in data.CommandCategories.Values)
                {
                    Console.WriteLine($"  {commandCategory.Name}:");
                    foreach (var command in commandCategory.Commands.Values)
                    {
                        PrintCommand(command, "  ");
                    }
                }
            }
            if (data.SettingCategories.Count != 0)
            {
                Console.WriteLine("Setting categories:");
                foreach (var settingCategory in data.SettingCategories.Values)
                {
                    Console.WriteLine($"  {settingCategory.Name}:");
                    foreach (var setting in settingCategory.Settings.Values)
                    {
                        PrintSetting(setting, "    ");
                    }
                }
            }
        }

        private static void PrintCommand(BaseCommand command, string ident)
        {
            Console.WriteLine($"{ident}  {command.Key}: {command.Name}");
            if (!string.IsNullOrEmpty(command.Description))
            {
                Console.WriteLine($"{ident}    Description:{command.Description}");
            }
            if (command.Parameters != null && command.Parameters.Length != 0)
            {
                Console.WriteLine($"{ident}    Params:");
                var paramIdent = ident + "    ";
                foreach (var parameter in command.Parameters)
                {
                    PrintParameter(parameter, paramIdent);
                }
            }
        }

        private static void PrintParameter(BaseParameter parameter, string ident)
        {
            Console.WriteLine($"{ident}  {parameter.Name}");
            if (!string.IsNullOrEmpty(parameter.Description))
            { 
                Console.WriteLine($"{ident}    {parameter.Description}");
            }
            if (parameter is IntegerParameter integerParameter)
            {
                Console.WriteLine($"{ident}    Default:{integerParameter.DefaultValue}");
                Console.WriteLine($"{ident}    Values:{string.Join(",", integerParameter.Values)}");
            }
        }

        private static void PrintSetting(BaseSetting setting, string ident)
        {
            Console.WriteLine($"{ident}{setting.Key}: {setting.Name}");
            if (!string.IsNullOrEmpty(setting.Description))
            {
                Console.WriteLine($"{ident}  Description:{setting.Description}");
            }
            if(setting is BooleanSetting booleanSetting)
            {
                Console.WriteLine($"{ident}  Default:{booleanSetting.DefaultValue}");
            }
            else if(setting is IntegerSetting integerSetting)
            {
                Console.WriteLine($"{ident}  Default:{integerSetting.DefaultValue}");
                Console.WriteLine($"{ident}  Values:{string.Join(",", integerSetting.Values)}");
            }
            else if (setting is FloatSetting floatSetting)
            {
                Console.WriteLine($"{ident}  Default:{floatSetting.DefaultValue}");
                Console.WriteLine($"{ident}  Values:{string.Join(",", floatSetting.Values)}");
            }
        }
    }
}
