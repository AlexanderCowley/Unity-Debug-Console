using System;
using UnityEngine;

namespace RuntimeDebugger.Commands
{
    public class CommandObject
    {
        public readonly object Instance;
        public readonly string InstanceKey;
        public readonly ConsoleCommand[] Commands;
        public ConsoleCommand LastCommand {get; private set;}
        public CommandObject(ConsoleCommand[] commands, string instanceKey, 
            object instance = null)
        {
            Instance = instance;
            InstanceKey = instanceKey;
            Commands = commands;
            CommandManager.Commands.Add(InstanceKey, this);
            Debug.Log($"Added Object: {InstanceKey}");
        }
        
        //Invoke command
        public void ProcessCommand(string commandKey = null, string[] args = null)
        {
            ConsoleCommand command = GetCommand(commandKey);
            command?.ProcessArgs(args);
            LastCommand = command;
        }

        ConsoleCommand GetCommand(string inputCommand)
        {
            int index = Array.IndexOf(Commands, inputCommand);

            if(index == -1)
            {
                Debug.LogWarning("Incorrect Command Entered");
                return null;
            }
            return Commands[index];
        }

        public void AddCommand(ConsoleCommand command)
        {
            //Add Command to instance
        }

        public void AddCommands(ConsoleCommand[] commands)
        {
            Debug.Log($"Commands added to {InstanceKey}");
        }
    }
}