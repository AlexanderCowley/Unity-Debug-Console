using UnityEngine;
using System;
public sealed class ConsoleCommand
{
    string _command;
    string _commandDescription;

    public ConsoleCommand(string commandName, string commandDescription)
    {
        this._command = commandName;
        this._commandDescription = commandDescription;
        CommandManager.Commands.Add(commandName, this);
    }
}
