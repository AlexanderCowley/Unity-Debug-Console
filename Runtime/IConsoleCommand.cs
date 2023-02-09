using System;

namespace RuntimeDebugger
{
    public interface IConsoleCommand
    {
        public string CommandKey{ get; }
        public string Description { get; }
        public void InvokeCommand();
        public void InvokeCommand(object[] args);
        public void ProcessArgs(string[] args = null);
    }

    public interface IConsoleCommand<T> : IConsoleCommand
        where T : IConvertible
    {
        public void InvokeCommand(T Value);
    }
}

