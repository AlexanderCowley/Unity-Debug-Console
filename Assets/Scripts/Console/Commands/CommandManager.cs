using UnityEngine;
using System;
using System.Collections.Generic;
public static class CommandManager
{
    public static Dictionary<string, IConsoleCommand> Commands = new Dictionary<string, IConsoleCommand>();

    public static IConsoleCommand LastCommand { get; private set; }

    public static void AddCommand(string commandTitle, string commandDescription, Action command)
    {
        if (Commands.ContainsKey(commandTitle))
        {
            Debug.LogWarning($"Command already in Command Dictionary");
            return;
        }
        
        new ConsoleCommand(commandTitle, commandDescription, command);
    }

    public static void AddCommand<T>(string commandTitle, string commandDescription, 
        Action<T> command) where T : IConvertible
    {
        if (Commands.ContainsKey(commandTitle))
        {
            Debug.LogWarning($"Command already in Command Dictionary");
            return;
        }

        new ConsoleCommand<T>(commandTitle, commandDescription, command);
    }

    public static void ParseCommand(string input)
    {
        //Seperate by space
        string[] inputProperties = input.Split(' ');
        for(int i = 0; i<inputProperties.Length; i++)
        {
            Debug.Log(inputProperties[i]);
        }
        //try/catch?
        //Log the Error to User
        if (!Commands.ContainsKey(inputProperties[0]))
        {
            Debug.LogWarning($"Command: {inputProperties[0]} not found");
            return;
        }

        Commands[inputProperties[0]]?.ProcessArgs(inputProperties);
    }

}
