using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Defter.StarCitizen.ConfigDB;
using Defter.StarCitizen.ConfigDB.Collection;
using Defter.StarCitizen.ConfigDB.Model;

namespace Defter.StarCitizen.TestApplication
{
    public class Program
    {
        public sealed class CommandHandler
        {
            private readonly LocalDatabaseManager _localDbManager;

            public CommandHandler(LocalDatabaseManager localDbManager)
            {
                _localDbManager = localDbManager;
            }

            public bool HandleCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "init":
                        return _localDbManager.Init();
                    case "load":
                        return CheckArgsCount(args, commandName, 1) &&
                            HandleLoadCommand(args[0], ArrayHelper.SubArray(args, 1));
                    case "unload":
                        return _localDbManager.Unload();
                    case "save":
                        return _localDbManager.Save();
                    case "print":
                        var data = _localDbManager.GetData();
                        if (data != null)
                        {
                            PrintConfigData(data);
                            return true;
                        }
                        Console.WriteLine("Error: Database not loaded");
                        return false;
                    case "lang":
                        return CheckArgsCount(args, commandName, 1) &&
                            HandleLanguageCommand(args[0], ArrayHelper.SubArray(args, 1));
                    case "command":
                        return CheckArgsCount(args, commandName, 1) &&
                            HandleCommandCommand(args[0], ArrayHelper.SubArray(args, 1));
                    case "setting":
                        return CheckArgsCount(args, commandName, 1) &&
                           HandleSettingCommand(args[0], ArrayHelper.SubArray(args, 1));
                    case "verify":
                        return CheckArgsCount(args, commandName, 1) &&
                            HandleVerifyCommand(args[0], ArrayHelper.SubArray(args, 1));
                    case "clear":
                        Console.Clear();
                        return false;
                    case "exit":
                        Environment.Exit(0);
                        return false;
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("init");
                        Console.WriteLine("load <command>");
                        Console.WriteLine("unload");
                        Console.WriteLine("save");
                        Console.WriteLine("print");
                        Console.WriteLine("lang <command>");
                        Console.WriteLine("command <command>");
                        Console.WriteLine("setting <command>");
                        Console.WriteLine("clear");
                        Console.WriteLine("exit");
                        return false;
                    default:
                        Console.WriteLine("Error: Unknown command - " + commandName);
                        return false;
                }
            }

            public bool HandleLoadCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "local":
                        _localDbManager.LoadLocal();
                        return true;
                    case "network":
                        _localDbManager.LoadNetwork();
                        return true;
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("load local");
                        Console.WriteLine("load network");
                        return false;
                    default:
                        Console.WriteLine("Error: Unknown load command - " + commandName);
                        return false;
                }
            }

            public bool HandleLanguageCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "create":
                        return CheckArgsCount(args, commandName, 1) &&
                            _localDbManager.CreateLanguage(args[0]);
                    case "add":
                        return CheckArgsCount(args, commandName, 1) &&
                            _localDbManager.AddLanguage(args[0]);
                    case "remove":
                        return CheckArgsCount(args, commandName, 1) &&
                            _localDbManager.RemoveLanguage(args[0], false);
                    case "delete":
                        return CheckArgsCount(args, commandName, 1) &&
                            _localDbManager.RemoveLanguage(args[0], true);
                    case "update":
                        return CheckArgsCount(args, commandName, 1) &&
                            _localDbManager.UpdateLanguage(args[0]);
                    case "print":
                        if (CheckArgsCount(args, commandName, 1))
                        {
                            var languageData = _localDbManager.GetLanguageData(args[0]);
                            if (languageData != null)
                            {
                                PrintConfigData(languageData);
                                return true;
                            }
                        }
                        return false;
                    case "list":
                        var languages = _localDbManager.GetSupportedLanguages();
                        if (languages != null)
                        {
                            Console.WriteLine("Available languages: " + string.Join(",", languages));
                            return true;
                        }
                        return false;
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("lang add <name>");
                        Console.WriteLine("lang create <name>");
                        Console.WriteLine("lang remove <name>");
                        Console.WriteLine("lang delete <name>");
                        Console.WriteLine("lang update <name>");
                        Console.WriteLine("lang print <name>");
                        Console.WriteLine("lang list");
                        return false;
                    default:
                        Console.WriteLine("Error: Unknown language command - " + commandName);
                        return false;
                }
            }

            public bool HandleCommandCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "categories":
                        {
                            var data = _localDbManager.GetData();
                            if (data != null)
                            {
                                Console.WriteLine("Available categories: " + string.Join(",", data.CommandCategories.Keys));
                                return true;
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    case "list":
                        {
                            var data = _localDbManager.GetData();
                            if (data != null)
                            {
                                Console.WriteLine("Available commands:");
                                Console.WriteLine(string.Join("\n", data.CommandCategories.Values.SelectMany(c => c.Commands.Keys)));
                                return true;
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    default:
                        Console.WriteLine("Error: Unknown command command - " + commandName);
                        return false;
                }
            }

            public bool HandleSettingCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "categories":
                        {
                            var data = _localDbManager.GetData();
                            if (data != null)
                            {
                                Console.WriteLine("Available categories: " + string.Join(",", data.SettingCategories.Keys));
                                return true;
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    case "list":
                        {
                            var data = _localDbManager.GetData();
                            if (data != null)
                            {
                                Console.WriteLine("Available settings:");
                                Console.WriteLine(string.Join("\n", data.SettingCategories.Values.SelectMany(c => c.Settings.Keys)));
                                return true;
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    default:
                        Console.WriteLine("Error: Unknown setting command - " + commandName);
                        return false;
                }
            }

            public bool HandleVerifyCommand(string commandName, string[] args)
            {
                switch (commandName)
                {
                    case "settings":
                        {
                            var configData = _localDbManager.GetData();
                            if (configData != null)
                            {
                                var settings = configData.SettingCategories.Values.SelectMany(c => c.Settings.Keys);
                                return DoVerifyStringsExist(new HashSet<string>(settings, StringComparer.OrdinalIgnoreCase));
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    case "commands":
                        {
                            var configData = _localDbManager.GetData();
                            if (configData != null)
                            {
                                var commands = configData.CommandCategories.Values.SelectMany(c => c.Commands.Keys);
                                return DoVerifyStringsExist(new HashSet<string>(commands, StringComparer.OrdinalIgnoreCase));
                            }
                            Console.WriteLine("Error: Database not loaded");
                            return false;
                        }
                    case "exist":
                        return DoVerifyStringsExist(new HashSet<string>(args, StringComparer.OrdinalIgnoreCase));
                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine("verify settings");
                        Console.WriteLine("verify commands");
                        Console.WriteLine("verify exist <name> [name] ...");
                        return false;
                    default:
                        Console.WriteLine("Error: Unknown verify command - " + commandName);
                        return false;
                }
            }

            private static byte[]? _verifyData;

            private static bool DoVerifyStringsExist(ISet<string> strings)
            {
                if (strings.Count != 0)
                {
                    if (_verifyData == null)
                    {
                        try
                        {
                            _verifyData = File.ReadAllBytes(Environment.GetEnvironmentVariable("LOCAL_GAME_EXECUTABLE"));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.ToString());
                            return false;
                        }
                    }
                    var accumulator = new StringBuilder(1024);
                    foreach (sbyte ch in _verifyData)
                    {
                        if (ch != 0)
                        {
                            accumulator.Append((char)ch);
                        }
                        else if (accumulator.Length != 0)
                        {
                            if (strings.Remove(accumulator.ToString()))
                            {
                                Console.WriteLine("FOUND: " + accumulator.ToString());
                                if (strings.Count == 0)
                                {
                                    return true;
                                }
                            }
                            accumulator.Clear();
                        }
                    }
                    if (accumulator.Length != 0 && strings.Remove(accumulator.ToString()))
                    {
                        Console.WriteLine("FOUND: " + accumulator.ToString());
                    }
                    if (strings.Count != 0)
                    {
                        foreach (var settingName in strings)
                        {
                            Console.WriteLine("MISSING: " + settingName);
                        }
                        Console.WriteLine("Error: missing strings found");
                        return false;
                    }
                }
                return true;
            }

            private static bool CheckArgsCount(string[] args, string commandName, int count)
            {
                if (args.Length < count)
                {
                    Console.WriteLine($"Error: Expected {count} arguments for {commandName}, but only {args.Length} provided");
                    return false;
                }
                return true;
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
                    Console.WriteLine($"{ident}    Description: {command.Description}");
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
                    Console.WriteLine($"{ident}    Default: {integerParameter.DefaultValue}");
                    Console.WriteLine($"{ident}    Values: {string.Join(",", integerParameter.Values)}");
                }
                else if (parameter is StringParameter stringParameter)
                {
                    Console.WriteLine($"{ident}    Default: {stringParameter.DefaultValue}");
                    Console.WriteLine($"{ident}    Values: {string.Join(",", stringParameter.Values)}");
                }
                else
                {
                    Console.WriteLine($"{ident}    Unknown: {parameter}");
                }
            }

            private static void PrintSetting(BaseSetting setting, string ident)
            {
                Console.WriteLine($"{ident}{setting.Key}: {setting.Name}");
                if (!string.IsNullOrEmpty(setting.Description))
                {
                    Console.WriteLine($"{ident}  Description: {setting.Description}");
                }
                if (setting is BooleanSetting booleanSetting && booleanSetting.DefaultValue.HasValue)
                {
                    Console.WriteLine($"{ident}  Default: {booleanSetting.DefaultValue.Value}");
                }
                else if (setting is IntegerSetting integerSetting && integerSetting.DefaultValue.HasValue)
                {
                    Console.WriteLine($"{ident}  Default: {integerSetting.DefaultValue.Value}");
                    Console.WriteLine($"{ident}  Values: {string.Join(",", integerSetting.Values)}");
                    if (integerSetting.Range)
                    {
                        Console.WriteLine($"{ident}  Range: {integerSetting.Range}");
                        Console.WriteLine($"{ident}  Min: {integerSetting.MinValue}");
                        Console.WriteLine($"{ident}  Max: {integerSetting.MaxValue}");
                        if (integerSetting.Step.HasValue)
                        {
                            Console.WriteLine($"{ident}  Step: {integerSetting.Step}");
                        }
                    }
                }
                else if (setting is FloatSetting floatSetting && floatSetting.DefaultValue.HasValue)
                {
                    Console.WriteLine($"{ident}  Default: {floatSetting.DefaultValue.Value}");
                    Console.WriteLine($"{ident}  Values: {string.Join(",", floatSetting.Values)}");
                    if (floatSetting.Range)
                    {
                        Console.WriteLine($"{ident}  Range: {floatSetting.Range}");
                        Console.WriteLine($"{ident}  Min: {floatSetting.MinValue}");
                        Console.WriteLine($"{ident}  Max: {floatSetting.MaxValue}");
                        if (floatSetting.Step.HasValue)
                        {
                            Console.WriteLine($"{ident}  Step: {floatSetting.Step}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{ident}    Unknown: {setting}");
                }
            }
        }

        public sealed class ConsoleErrorOutput : IErrorOutput
        {
            public void WriteLine(string value) => Console.WriteLine(value);
        }

        private static void Main(string[] args)
        {
            var splitChars = new char[] { ' ' };
            var fileSourceSettings = new LocalFileSourceSettings(args.Length > 0 ? args[0] : Environment.CurrentDirectory);
            var networkSourceSettings = new GitHubSourceSettings();
            // var networkSourceSettings = new GiteeSourceSettings();
            var localDbManager = new LocalDatabaseManager(networkSourceSettings, fileSourceSettings, new ConsoleErrorOutput());
            var commandHandler = new CommandHandler(localDbManager);
            while (true)
            {
                var argsuments = Console.ReadLine().Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                if (argsuments.Length > 0)
                {
                    if (commandHandler.HandleCommand(argsuments[0], ArrayHelper.SubArray(argsuments, 1)))
                    {
                        Console.WriteLine("Done");
                    }
                }
            }
        }
    }
}
