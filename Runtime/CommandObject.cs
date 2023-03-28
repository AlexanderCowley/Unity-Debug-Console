using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDebugger.Commands
{
    public class CommandObject
    {
        //Should refer to gameobject if the script is a monobehaviour
        public readonly object Instance;
        public string InstanceKey {get; private set;}
        public List<ConsoleCommand> Commands {get; private set;} = new();
        public ConsoleCommand LastCommand {get; private set;}

        //Static Utility Constructor
        public CommandObject(ConsoleCommand command, string instanceKey, 
            object instance = null)
        {
            Instance = instance;
            InstanceKey = instanceKey;
            AddCommand(command, instance);
            CommandManager.Commands.Add(InstanceKey, this);
        }
        //Monobehaviour instance with a command
        public CommandObject(ConsoleCommand command, string instanceKey, 
            MonoBehaviour instance)
        {
            Instance = instance.gameObject;
            InstanceKey = instanceKey;
            AddCommand(command, instance);
            CommandManager.Commands.Add(InstanceKey, this);
        }
        //Static Methods with no commands
        public CommandObject(string instanceKey, object instance = null)
        {
            InstanceKey = instanceKey;
            Instance = instance;
            CommandManager.Commands.Add(InstanceKey, this);
        }
        //Used for copying CommandObjects from other CommandObjects
        public CommandObject(string instanceKey, GameObject instance)
        {
            InstanceKey = instanceKey;
            Instance = instance;
            Debug.Log("Copy Generated " + InstanceKey);
            CommandManager.Commands.Add(InstanceKey, this);
        }
        
        //Command can be found and invoked
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

        public void SetInstanceKey(string newKey)
        {
            InstanceKey = newKey;
        }
    }
}