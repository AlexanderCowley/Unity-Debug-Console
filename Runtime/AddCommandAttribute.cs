using System;
namespace RuntimeDebugger.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AddCommandAttribute : Attribute
    {
        public string CmdName {get; set;}
        public string CmdDesc {get; set;}
        public AddCommandAttribute(string commandName, string commandDescription = "")
        {
            this.CmdName = commandName;
            this.CmdDesc = commandDescription;
        }
    }
}

