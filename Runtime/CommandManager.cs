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
    /// to invoke a delegate stored in a command object.
    /// </summary>
    public static class CommandManager
    {
        public static Dictionary<string, CommandObject> Commands = 
            new Dictionary<string, CommandObject>();
        public static CommandObject LastCMDInstance { get; private set; }

        public static List<string> InputCommandLogs = new();
        static List<Type> RegisteredTypes = new();
        
        static AddCommandAttribute _cmdAttr;
        static Assembly _currentAssembly;
        public static void AddAllCommands()
        {
            //Iterate through each assembly to find assemblies with the AddCommandAttribute
            //Get Monobehaviors
            //Assembly -> Types -> Methods -> Methods with Attribute
            //Assembly -> Types -> instances -> methods with attribute
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
            //Isolate assemblies
            for(int i = 0; i < assemblies.Count(); i++)
            {
                _currentAssembly = assemblies.ElementAt(i);
                Type[] types = _currentAssembly.GetTypes();

                //Switch to memberinfo when adding property and field support
                for(int j = 0; j < types.Length; j++)
                {
                    MethodInfo[] methods = types[j].
                        GetMethods(BindingFlags.Public | 
                        BindingFlags.NonPublic | BindingFlags.Static |
                        BindingFlags.Instance);

                    var monoFiltered = monoBehaviours.Where(x => 
                        _currentAssembly.GetType(
                        x.GetType().FullName) == types[j]);
                    
                    if(!monoFiltered.Any())
                        continue;

                    //Matches multiple objects with the same component
                    for (int m = 0; m < monoFiltered.Count(); m++)
                    {
                        //Adding commands for one type
                        for(int k = 0; k < methods.Length; k++)
                        {
                            //Check types for attribute
                            _cmdAttr = methods[k].GetCustomAttribute<AddCommandAttribute>();
                            if(_cmdAttr == null)
                                continue;
                            
                            //Get Delegate type
                            Delegate cmd = CreateDelegate(methods[k], monoFiltered.ElementAt(m));
                            AddCommand(new ConsoleCommand(_cmdAttr.CmdName, 
                                _cmdAttr.CmdDesc, cmd), monoFiltered.ElementAt(m));
                        }
                    }
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

                CommandObject staticCommands = new CommandObject(string.Empty);

                for(int i = 0; i < UtilityCommands.Count(); i++)
                {
                    _cmdAttr = UtilityCommands.ElementAt(i).
                        GetCustomAttribute<AddCommandAttribute>();

                    if(_cmdAttr == null)
                        continue;
                    
                    Delegate cmd = CreateDelegate(UtilityCommands.ElementAt(i), null);
                    staticCommands.AddCommand(new ConsoleCommand(_cmdAttr.CmdName, 
                        _cmdAttr.CmdDesc, cmd), null);
                }
            }

        static void AddCommand(ConsoleCommand command, MonoBehaviour monoScript)
        {
            //Cache key
            string key = string.Empty;
            Dictionary<string, CommandObject>.ValueCollection commandObjects = Commands.Values;
            for(int i = 0; i < commandObjects.Count(); i++)
            {
                object instance = commandObjects.ElementAt(i).Instance;
                //First CommandObject has no instance
                //If instance does not exist add commands to static commandObject
                if(monoScript == null)
                {
                    Commands[string.Empty].AddCommand(command, null);
                    continue;
                }
                //If the instance is null continue the loop
                if(instance == null)
                {
                    continue;
                }
                //If the instance and monobehaviour pointer are the same
                //then add the commands to the CommandObject
                if(Object.ReferenceEquals(instance, monoScript.gameObject))
                {
                    //Get Instance key
                    key = commandObjects.ElementAt(i).InstanceKey;
                    //Add Commands to object
                    Commands[key].AddCommand(command, monoScript);
                    return;
                }
            }
            //If the key is not found add a new command object
            string generatedKey = GenerateInstanceKey(monoScript);
            if(generatedKey == null)
            {
                Debug.LogWarning("Duplicate Command Entered");
                return;
            }
            new CommandObject(command, generatedKey, monoScript);
        }

        static void AddCommand(ConsoleCommand command, object instance)
        {
            //Cache key
            string key = string.Empty;
            Dictionary<string, CommandObject>.ValueCollection commandObjects = Commands.Values;
            for(int i = 0; i < commandObjects.Count(); i++)
            {
                object currrentInstance = commandObjects.ElementAt(i).Instance;
                //First CommandObject has no instance
                //If instance does not exist add commands to static commandObject
                if(instance == null)
                {
                    Commands[string.Empty].AddCommand(command, null);
                    continue;
                }
                //If the instance is null continue the loop
                if(currrentInstance == null)
                {
                    continue;
                }
                //If the instance and monobehaviour pointer are the same
                //then add the commands to the CommandObject
                if(Object.ReferenceEquals(currrentInstance, instance))
                {
                    //Get Instance key
                    key = commandObjects.ElementAt(i).InstanceKey;
                    //Add Commands to object
                    Commands[key].AddCommand(command, instance);
                    return;
                }
            }
            //If the key is not found add a new command object
            string generatedKey = GenerateInstanceKey(instance);
            if(generatedKey == null)
            {
                Debug.LogWarning("Duplicate Command Entered");
                return;
            }
            new CommandObject(command, generatedKey, instance);
        }
        
        //Call in constructor
        //Switch parameters to make instance optional for static methods
        public static void RegisterType(object instance, string instanceKey, Type type)
        {
            //Get Methods
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Static |
                BindingFlags.Instance);

            //Get Method Attributes with AddCommand
            for(int i = 0; i < methods.Count(); i++)
            {
                _cmdAttr = methods.ElementAt(i).
                    GetCustomAttribute<AddCommandAttribute>();

                if(_cmdAttr == null)
                    continue;

                Delegate cmd = CreateDelegate(methods.ElementAt(i), instance);
                //Add command
                AddCommand(new ConsoleCommand(_cmdAttr.CmdName, 
                    _cmdAttr.CmdDesc, cmd), instance);
            }
            RegisteredTypes.Add(type);
        }

        public static void UnregisterType(Type type)
        {
            RegisteredTypes.Remove(type);
            //Remove Command
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

        static string GenerateInstanceKey(MonoBehaviour monoScript)
        {
            if(monoScript == null)
                return null;

            string key = monoScript.gameObject.name;
            if(Commands.ContainsKey(key))
            {
                Debug.LogWarning("Key already exists");
                return null;
            }

            int keyIndex = key.IndexOf(" (");
            int increment = 1;

            //A key exists within the dictionary that has no spaces
            //If there is no space
            if(keyIndex == -1)
            {
                if(!Commands.ContainsKey(key))
                {
                    //A key with no spaces and is not in the dictionary
                    return key;
                }

                //A key with no spaces and is in the dictionary
                return null;
            }

            //There is a <space(>
            key = key.Substring(0, keyIndex);
            while(Commands.ContainsKey($"{key}-{increment}"))
            {
                increment++;
            }
            return $"{key}-{increment}";
        }

        static string GenerateInstanceKey(object instance)
        {
            if(instance == null)
                return null;

            string key = instance.GetType().Name;
            int increment = 1;
            //GetType.Name does not have duplicate iterator so
            //just increment as the subject is by itself
            while(Commands.ContainsKey($"{key}-{increment}"))
            {
                increment++;
            }
            return $"{key}-{increment}";
        }

        public static void ParseCommand(string input)
        {
            int commandKeyIndex = 0;
            int instanceKeyIndex = 0;
            string instanceKey = string.Empty;
            string commandKey;
            //Remove space from beginning and end of input
            input.Trim();

            //Check for . to indicate instance key
            instanceKeyIndex = input.IndexOf('.');
            
            if(instanceKeyIndex != -1)
            {
                instanceKey = input.Substring(0, instanceKeyIndex);
                input = input.Substring(instanceKeyIndex + 1);
            }
            //Seperate key by space
            //if the input has no white space then, there are no args
            //Add support for optional parameters??
            if(!input.Contains(' '))
            {
                commandKey = input;

                if (!Commands.ContainsKey(instanceKey))
                {
                    InputCommandLogs.Add($"Command: {instanceKey} does not exist.");
                    InputCommandLogs.Add($"Type help for a list of available commands.");
                    return;
                }

                Commands[instanceKey]?.ProcessCommand(commandKey);
                LastCMDInstance = Commands[instanceKey];
                return;
            }
            //Get Key
            commandKeyIndex = input.IndexOf(' ');
            commandKey = input.Substring(0, commandKeyIndex);
            string inputParameters = input.Substring(commandKeyIndex + 1);
            string[] inputProperties = inputParameters.Split(' ');
            //try/catch?
            //Log the Error to User
            if (!Commands.ContainsKey(instanceKey))
            {
                InputCommandLogs.Add($"Command: {instanceKey} does not exist.");
                InputCommandLogs.Add($"Type help for a list of available commands.");
                return;
            }

            Commands[instanceKey]?.ProcessCommand(commandKey, inputProperties);
            LastCMDInstance = Commands[instanceKey];
        }
    }
}