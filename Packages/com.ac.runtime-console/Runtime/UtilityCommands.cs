using UnityEngine;

namespace RuntimeDebugger.Commands
{
    public class UtilityCommands
    {
        public UtilityCommands()
        {
            CommandManager.AddCommand("help", "displays all commands", Help);
        }

        void Help()
        {
            Debug.Log("Display stuff");
            //List all commands;
        }
    }
}

