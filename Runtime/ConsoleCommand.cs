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

        public string Description => _commandDescription;
        public ConsoleCommand(string commandTitle, string commandDescription, 
        Delegate command, object instance)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._command = command;

            //Get parameter types
            ParamTypes = command.GetMethodInfo().GetParameters();
            _instance = instance;
            CommandManager.Commands.Add(commandTitle, this);
        }

        public ConsoleCommand(string commandTitle, string commandDescription, 
        Delegate command)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._command = command;

            //Get parameter types
            ParamTypes = command.GetMethodInfo().GetParameters();
            CommandManager.Commands.Add(commandTitle, this);
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
                CommandManager.InputCommandLogs.Add($"{_commandTitle}" + 
                    "takes in these parameter(s) => \n\t" + 
                    $"{string.Join("\n\t", (object[])ParamTypes)}");
                return;
            }

            var message = _command?.GetMethodInfo().Invoke(_instance, null);

            if(!_command.GetMethodInfo().ReturnType.Equals(typeof(void)))
                CommandManager.InputCommandLogs.Add(message.ToString());
        }

        public void ProcessArgs(string[] args)
        {
            //
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
            //parameters
            InvokeCommand(parameters);
        }
    }

    public class ConsoleCommand<T> : IConsoleCommand<T> where T : IConvertible
    {
        string _commandTitle;
        string _commandDescription;
        Action<T> _command;
        Action<T,T> _commandTwoParams;
        Action<T,T,T> _commandThreeParams;
        public ParameterInfo[] ParamTypes { get; private set; }
        MethodInfo _commandInfo;
        object _sender;
        public ConsoleCommand(string commandTitle, string commandDescription, 
            Action<T> commandWithParam, object sender)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._command = commandWithParam;
            _sender = sender;
            
            //Get parameter array for matching types
            _commandInfo = _command.GetMethodInfo();
            ParamTypes = _commandInfo.GetParameters();
            CommandManager.Commands.Add(commandTitle, this);
        }

        public ConsoleCommand(string commandTitle, string commandDescription, 
            Action<T,T> commandWithParam, object sender)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._commandTwoParams = commandWithParam;
            _sender = sender;
            
            //Get parameter array for matching types
            _commandInfo = _commandTwoParams.GetMethodInfo();
            ParamTypes = _commandInfo.GetParameters();
            CommandManager.Commands.Add(commandTitle, this);
        }

            public ConsoleCommand(string commandTitle, string commandDescription, 
                Action<T,T,T> commandWithParam, object sender)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._commandThreeParams = commandWithParam;
            _sender = sender;
            
            //Get parameter array for matching types
            _commandInfo = _commandThreeParams.GetMethodInfo();
            ParamTypes = _commandInfo.GetParameters();
            CommandManager.Commands.Add(commandTitle, this);
        }

        public string Description => _commandDescription;

        public void InvokeCommand(object[] Values)
        {
            _commandInfo?.Invoke(_sender, Values);
        }

        public void InvokeCommand(T Value){}

        //If no parameters are found call this
        //Set to default value
        public void InvokeCommand()
        {
            Debug.LogWarning("No Parameter");
        }

        public void ProcessArgs(string[] args)
        {
            if (args.Length - 1 != ParamTypes.Length)
            {
                Debug.LogWarning("Incorrect number of parameters");
                return;
            }

            object[] parameters = new object[ParamTypes.Length];
            //Match type conversion with ParamType Array
            for(int i = 1; i < ParamTypes.Length; i++)
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

            InvokeCommand(parameters);
        }
    }
}

