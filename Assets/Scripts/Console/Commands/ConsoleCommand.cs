using UnityEngine;
using System;
public class ConsoleCommand : IConsoleCommand
{
    string _commandTitle;
    string _commandDescription;
    Action _command;

    public string Description => _commandDescription;
    public ConsoleCommand(string commandName, string commandDescription, Action command)
    {
        this._commandTitle = commandName;
        this._commandDescription = commandDescription;
        this._command = command;
        CommandManager.Commands.Add(commandName, this);
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

public class ConsoleCommand<T> : IConsoleCommand<T> where T:IConvertible
{
    string _commandTitle;
    string _commandDescription;
    Action<T> _command;
    public Type ParamType { get; private set; }
    public ConsoleCommand(string commandTitle, string commandDescription, Action<T> commandWithParam)
    {
        this._commandTitle = commandTitle;
        this._commandDescription = commandDescription;
        this._command = commandWithParam;
        CommandManager.Commands.Add(commandTitle, this);
        ParamType = typeof(T);
    }

    public string Description => _commandDescription;

    public void InvokeCommand(T Value)
    {
        _command?.Invoke(Value);
    }

    //If no parameters are found call this
    //Set to default value
    public void InvokeCommand()
    {
        Debug.LogWarning("No Parameter");
    }

    public void ProcessArgs(string[] args)
    {
        if (args.Length - 1 > 1 || args.Length - 1 < 1)
        {
            Debug.LogWarning("Incorrect number of parameters");
            return;
        }

        //Parse to T type
        T parameterData;
        try
        {
            parameterData = (T)Convert.ChangeType(args[1], typeof(T));
        }
        catch (FormatException)
        {
            Debug.LogWarning("Incorrect Parameters");
            return;
        }

        InvokeCommand(parameterData);
    }
}
