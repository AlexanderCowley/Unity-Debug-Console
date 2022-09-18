using UnityEngine;
using System.Collections.Generic;
public sealed class CommandManager : MonoBehaviour
{
    public static Dictionary<string, ConsoleCommand> Commands = new Dictionary<string, ConsoleCommand>();

    public void ParseCommand(string command)
    {
        
    }
}
