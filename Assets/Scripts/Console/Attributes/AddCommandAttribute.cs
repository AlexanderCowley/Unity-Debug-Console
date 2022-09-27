using UnityEngine;
using System;
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class AddCommandAttribute : PropertyAttribute
{
    string _command;
    string _commandDescription;
    public AddCommandAttribute(string commandName, string commandDescription)
    {
        this._command = commandName;
        this._commandDescription = commandDescription;
        //TryGetCommand();
    }

    /*void TryGetCommand()
    {
        if(!CommandManager.Commands.ContainsKey(_command))
            new ConsoleCommand(_command, _commandDescription);
    }*/
}
