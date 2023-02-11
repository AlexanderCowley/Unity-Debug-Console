using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDebugger.Commands
{
    public class CommandObject
    {
        //Should refer to gameobject if the script is a monobehaviour
        public readonly object Instance;
        public readonly string InstanceKey;
        public List<ConsoleCommand> Commands { get; private set; } = new();
        public ConsoleCommand LastCommand {get; private set;}
        public CommandObject(ConsoleCommand command, string instanceKey, 
            object instance = null)
        {
            Instance = instance;
            InstanceKey = instanceKey;
            AddCommand(command, instance);
            CommandManager.Commands.Add(InstanceKey, this);
        }

        public CommandObject(ConsoleCommand command, string instanceKey, 
            MonoBehaviour instance)
        {
            Instance = instance.gameObject;
            InstanceKey = instanceKey;
            AddCommand(command, instance);
            CommandManager.Commands.Add(InstanceKey, this);
        }

        public CommandObject(string instanceKey, object instance = null)
        {
            InstanceKey = instanceKey;
            Instance = instance;
            CommandManager.Commands.Add(InstanceKey, this);
        }
        
        //Invoke command
        public void ProcessCommand(string commandKey, string[] args = null)
        {
            ConsoleCommand command = GetCommand(commandKey);
            command?.ProcessArgs(args);
            LastCommand = command;
        }

        ConsoleCommand GetCommand(string inputCommand)
        {
            ConsoleCommand command = Commands.Find(
                command => command.CommandKey == inputCommand);

            if(command == null)
            {
                Debug.LogWarning("Command Not Found");
                return null;
            }
            return command;
        }

        public void AddCommand(ConsoleCommand commandToAdd, object cmdInstance)
        {
            commandToAdd.SetInstance(cmdInstance);
            Commands.Add(commandToAdd);
        }
    }
}