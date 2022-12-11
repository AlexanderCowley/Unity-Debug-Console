using UnityEngine;
using System;
using System.Collections.Generic;

namespace RuntimeDebugger.Commands
{
    /// <summary>
    /// The Command Manager stores a dictionary of commands that can be matched
    /// to call an action delegate stored in a command object.
    /// </summary>
    public static class CommandManager
    {
        public static Dictionary<string, IConsoleCommand> Commands = 
            new Dictionary<string, IConsoleCommand>();
#if !UNITY_EDITOR
        public static LogHandler CommandLog = new LogHandler
        ("RuntimeConsoleLogs", "CommandList", ".txt", "Command Log");
        public static LogHandler CommandErrorLog = new LogHandler
            ("RuntimeConsoleLogs", "CommandErrors", ".txt", "Command Errors");
#endif
        public static IConsoleCommand LastCommand { get; private set; }

        public static List<string> InputCommandLogs = new List<string>();

        public static void AddCommand(string commandTitle, string commandDescription, Action command)
        {
            if (Commands.ContainsKey(commandTitle))
            {
                #if UNITY_EDITOR
                Debug.LogWarning($"{commandTitle} is already an existing command");
                #endif

                #if !UNITY_EDITOR
                CommandErrorLog.WriteToLog($"{commandTitle} is already an existing command");
                #endif
                return;
            }
            new ConsoleCommand(commandTitle, commandDescription, command);
        }

        public static void AddCommand<T>(string commandTitle, string commandDescription,
            Action<T> command, object sender) where T : IConvertible
        {
            if (Commands.ContainsKey(commandTitle))
            {
#if !UNITY_EDITOR
                CommandErrorLog.WriteToLog($"{commandTitle} already exists as an existing command");
#endif
                return;
            }

            new ConsoleCommand<T>(commandTitle, commandDescription, command, sender);
        }

        public static void AddCommand<T>(string commandTitle, string commandDescription,
            Action<T,T> command, object sender) where T : IConvertible
        {
            if (Commands.ContainsKey(commandTitle))
            {
#if !UNITY_EDITOR
                CommandErrorLog.WriteToLog($"{commandTitle} already exists as an existing command");
#endif
                return;
            }

            new ConsoleCommand<T>(commandTitle, commandDescription, command, sender);
        }

        public static void AddCommand<T>(string commandTitle, string commandDescription,
            Action<T,T,T> command, object sender) where T : IConvertible
        {
            if (Commands.ContainsKey(commandTitle))
            {
#if !UNITY_EDITOR
                CommandErrorLog.WriteToLog($"{commandTitle} already exists as an existing command");
#endif
                return;
            }

            new ConsoleCommand<T>(commandTitle, commandDescription, command, sender);
        }

        public static void ParseCommand(string input)
        {
            //Remove space from beginning of input
            input.TrimStart();
            //Seperate by space
            string[] inputProperties = input.Split(' ');
            //try/catch?
            //Log the Error to User
            if (!Commands.ContainsKey(inputProperties[0]))
            {
#if !UNITY_EDITOR
                CommandErrorLog.WriteToLog($"Command: {inputProperties[0]} does not exist.");
                CommandErrorLog.WriteToLog($"Type help for a list of available commands.");
#endif
                return;
            }
            Commands[inputProperties[0]]?.ProcessArgs(inputProperties);
            LastCommand = Commands[inputProperties[0]];
        }
    }
}

