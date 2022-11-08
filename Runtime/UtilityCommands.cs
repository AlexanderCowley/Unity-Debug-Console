using UnityEngine;

namespace RuntimeDebugger.Commands
{
    public static class UtilityCommands
    {
        public static void AddDefaultCommands()
        {
            CommandManager.AddCommand("help", "displays all commands", Help);
        }

        static void Help()
        {
            Debug.Log("Display stuff");
            //List all commands;
        }
    }
}

