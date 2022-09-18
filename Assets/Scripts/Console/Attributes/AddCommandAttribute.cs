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
        new ConsoleCommand(commandName, commandDescription);
    }

    //Add to a list of Commands
}
