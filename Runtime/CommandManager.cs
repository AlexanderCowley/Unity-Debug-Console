using UnityEngine;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

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
        public static IConsoleCommand LastCommand { get; private set; }

        public static List<string> InputCommandLogs = new List<string>();
        
        static AddCommandAttribute _cmdAttr;
        public static void AddAllCommands()
        {
            //Iterate through each assembly to find assemblies with the AddCommandAttribute
            //Assembly -> Types -> Methods -> Methods with Attribute
            //Assembly -> Types (Get Types with attribute, Get Instances), -> Methods
            //Assembly -> Types -> Types with attributes -> instances -> methods with attribute
            /*Where statements blacklist assemblies that starts with the name
            Unity, Microsoft and System, reduces the assembly to loop from 157 to 15
            in a mostly empty project that holds up to three assemblies for each tool directory
            */
            //Grabs utility commands
            GetUtilityCommands();
            //Get Monobehavior Instance
            MonoBehaviour[] monoBehaviours = GetMonoBehaviours(true);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().
            Where(x => !x.FullName.StartsWith("Unity") && 
            !x.FullName.StartsWith("Microsoft") &&
            !x.FullName.StartsWith("System"));
            for(int i = 0; i < assemblies.Count(); i++)
            {
                Type[] types = assemblies.ElementAt(i).GetTypes();
                //Switch to memberinfo when adding property and field support
                for(int j = 0; j < types.Length; j++)
                {
                    MethodInfo[] methods = types[j].
                        GetMethods(BindingFlags.Public | 
                        BindingFlags.NonPublic | BindingFlags.Static |
                        BindingFlags.Instance);
                    var monos = monoBehaviours.Where(x => x.GetType() == types[j]);

                    for(int k = 0; k < methods.Length; k++)
                    {
                        //Check types for attribute
                        _cmdAttr = methods[k].GetCustomAttribute<AddCommandAttribute>();
                        if(_cmdAttr == null)
                            continue;

                        for(int l = 0; l < monos.Count(); l++)
                        {
                            //Get Delegate type
                            Delegate cmd = CreateDelegate(methods[k], monos.ElementAt(l));
                            new ConsoleCommand(_cmdAttr.CmdName, _cmdAttr.CmdDesc, cmd, 
                            monos.ElementAt(l));
                        }
                        //Get object Instance from register object
                        //Remove object Instance from unregister object
                    }
                }
            }
            //Retrieve attribute for attribute data from parameters
            //Gather all methods with the AddCommand Attribute
            //Create Instance of new command
            //Create a new delegate the new command
            //Add them to the dictionary using the <command><methodInfo>

            static void GetUtilityCommands()
            {
            //Get utility commands
            var UtilityCommands = Assembly.
                GetExecutingAssembly().GetType("RuntimeDebugger.Commands.UtilityCommands").
                GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                for(int i = 0; i < UtilityCommands.Count(); i++)
                {
                    _cmdAttr = UtilityCommands.ElementAt(i).
                        GetCustomAttribute<AddCommandAttribute>();

                    if(_cmdAttr == null)
                        continue;

                    Delegate cmd = CreateDelegate(UtilityCommands.ElementAt(i), null);
                    new ConsoleCommand(_cmdAttr.CmdName, _cmdAttr.CmdDesc, cmd);
                }
            }
        }

        static Delegate CreateDelegate(MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            bool isAction = methodInfo.ReturnType.Equals(typeof(void));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);
            
            if(isAction) getType = Expression.GetActionType;
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[]{methodInfo.ReturnType});
            }

            if(methodInfo.IsStatic)
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);

            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);;
        }

        public static MonoBehaviour[] GetMonoBehaviours(bool allowInActive)
        {
            MonoBehaviour[] monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>(allowInActive);
            return monoBehaviours;
        }

        public static void AddCommand(string commandTitle, string commandDescription, Action command)
        {
            if (Commands.ContainsKey(commandTitle))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{commandTitle} is already an existing command");
#endif
                return;
            }
        }

        public static void AddCommand<T>(string commandTitle, string commandDescription,
            Action<T> command, object sender) where T : IConvertible
        {
            if (Commands.ContainsKey(commandTitle))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{commandTitle} is already an existing command");
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
#if UNITY_EDITOR
                Debug.LogWarning($"{commandTitle} is already an existing command");
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
#if UNITY_EDITOR
                Debug.LogWarning($"{commandTitle} is already an existing command");
#endif
                return;
            }

            new ConsoleCommand<T>(commandTitle, commandDescription, command, sender);
        }

        public static void ParseCommand(string input)
        {
            //Remove space from beginning of input
            input.Trim();
            //Seperate by space
            int commandKeyIndex = input.IndexOf(' ');
            string commandKey = input.Substring(0, commandKeyIndex);
            string inputParameters = input.Substring(commandKeyIndex + 1);
            string[] inputProperties = inputParameters.Split(' ');
            //try/catch?
            //Log the Error to User
            if (!Commands.ContainsKey(commandKey))
            {
                InputCommandLogs.Add($"Command: {commandKey} does not exist.");
                InputCommandLogs.Add($"Type help for a list of available commands.");
                return;
            }
            Commands[commandKey]?.ProcessArgs(inputProperties);
            LastCommand = Commands[commandKey];
        }
    }
}

