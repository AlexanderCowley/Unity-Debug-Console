using System;
public interface IConsoleCommand
{
    public string Description { get; }
    public void InvokeCommand();
    public void ProcessArgs(string[] args);
}

public interface IConsoleCommand<T> : IConsoleCommand 
    where T : IConvertible
{
    public void InvokeCommand(T Value);
}
