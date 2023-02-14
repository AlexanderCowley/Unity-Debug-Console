using System.Collections.Generic;
namespace RuntimeDebugger.Commands
{
    public static class UtilityCommands
    {
        //Lists all commands
        [AddCommand("help")]
        static void Help()
        {
            CommandManager.InputCommandLogs.Add("Commands Available: ");
            Dictionary<string, CommandObject>.KeyCollection keys = CommandManager.Commands.Keys;
            foreach(string key in keys)
            {
                if(key == string.Empty)
                    continue;
                CommandManager.InputCommandLogs.Add($" - {key}: " + 
                    $"{CommandManager.Commands[key]?.LastCommand?.Description}");
            }
                
        }
        [AddCommand("clear")]
        static void ClearLog()
        {
            CommandManager.InputCommandLogs.Clear();
        }
    }
}

