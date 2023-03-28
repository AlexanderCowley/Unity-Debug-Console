using UnityEngine;
using System;
using System.Reflection;

namespace RuntimeDebugger.Commands
{
    public class ConsoleCommand : IConsoleCommand
    {
        string _commandTitle;
        string _commandDescription;
        Delegate _command;
        public ParameterInfo[] ParamTypes { get; private set; }
        object _instance;

        public string CommandKey => _commandTitle;
        public string Description => _commandDescription;
        public ConsoleCommand(string commandTitle, 
            string commandDescription, Delegate command)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._command = command;

            //Get parameter types
            ParamTypes = command.GetMethodInfo().GetParameters();
        }
        //Set instance for Commands
        public void SetInstance(object instance)
        {
            _instance = instance;
        }
        
        public void InvokeCommand(object[] args)
        {
            var message = _command?.GetMethodInfo().Invoke(_instance, args);

            if(!_command.GetMethodInfo().ReturnType.Equals(typeof(void)))
                CommandManager.InputCommandLogs.Add(message.ToString());
        }

        public void InvokeCommand()
        {
            if(ParamTypes.Length != 0)
            {
                CommandManager.InputCommandLogs.Add("Incorrect parameters");
                //Alt: Use a for loop for each type with Type.Name field
                //Might not be efficent but, it will be clear
                //Memory Issue?
                CommandManager.InputCommandLogs.Add($"{_commandTitle} " + 
                    "takes in these parameter(s) => \n\t" + 
                    $"{string.Join("\n\t", (object[])ParamTypes)}");
                return;
            }

            var message = _command?.GetMethodInfo().Invoke(_instance, null);

            if(!_command.GetMethodInfo().ReturnType.Equals(typeof(void)))
                CommandManager.InputCommandLogs.Add(message.ToString());
        }

        public void ProcessArgs(string[] args = null)
        {
            //Checks if there are parameters to convert
            if(args == null)
            {
                InvokeCommand();
                return;
            }

            //If the amount of parameters is not equal to the argument length
            if (args.Length != ParamTypes.Length)
            {
                Debug.LogWarning("Incorrect number of parameters");
                return;
            }

            object[] parameters = new object[ParamTypes.Length];
            //Match type conversion with ParamType Array
            for(int i = 0; i < ParamTypes.Length; i++)
            {
                try
                {
                    parameters[i] = Convert.ChangeType(args[i], ParamTypes[i].ParameterType);
                }
                catch(FormatException)
                {
                    Debug.LogWarning("Incorrect Parameters");
                    return;
                }
                
            }
            //input parameters to invoke command
            InvokeCommand(parameters);
        }
    }
}

