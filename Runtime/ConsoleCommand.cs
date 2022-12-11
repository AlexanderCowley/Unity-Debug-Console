using UnityEngine;
using System;
using System.Reflection;

namespace RuntimeDebugger.Commands
{
    public class ConsoleCommand : IConsoleCommand
    {
        string _commandTitle;
        string _commandDescription;
        Action _command;

        public string Description => _commandDescription;
        public ConsoleCommand(string commandTitle, string commandDescription, Action command)
        {
            this._commandTitle = commandTitle;
            this._commandDescription = commandDescription;
            this._command = command;
            CommandManager.Commands.Add(commandTitle, this);

            #if !UNITY_EDITOR
            CommandManager.CommandLog.WriteToLog($"Command: {commandTitle}");
            #endif
        }

        public void InvokeCommand()
        {
            _command?.Invoke();
        }

        public void ProcessArgs(string[] args)
        {
            InvokeCommand();
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

