using System.Collections.Generic;
namespace RuntimeDebugger.Commands
{
    public static class UtilityCommands
    {
        public static void AddDefaultCommands()
        {
            CommandManager.AddCommand("help", "displays all commands", Help);
            CommandManager.AddCommand("clear", "clears log history", ClearLog);
        }
        //Lists all commands
        static void Help()
        {
            CommandManager.InputCommandLogs.Add("Commands Available: ");
            Dictionary<string, IConsoleCommand>.KeyCollection keys = CommandManager.Commands.Keys;
            foreach(string key in keys)
                CommandManager.InputCommandLogs.Add($" - {key}: " + 
                    $"{CommandManager.Commands[key].Description}");
        }

        static void ClearLog()
        {
            CommandManager.InputCommandLogs.Clear();
        }
    }
}

