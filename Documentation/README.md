Runtime Console

Summary: Runtime Console is a tool for Unity that generates a 
	console or cheat menu that allows users to run methods/functions
	while in a build of their project.

Input: The Console Menu is set to Toggle with the (`) Key.

Instructions:

Adding a Command:

	1. Be sure to add the ConsoleMenu component to a gameobject.
	2. Add using RuntimeDebugger.Commands to your scripts.
	3. Create a function/method that is of type void and one
		parameter type.
	4. Write: CommandManager.AddCommand(string commandTitle, 
		string commandDescription, MethodName);

	5. While in playmode, press the ` key to open the console
		then type in the commandTitle that was input above to
		call that method.

	NOTE: 
	Acceptable parameter types: object, int, float, string, bool. 
	Or more specifically anything that implement IConvertiable.

Adding a Log:
	1. Add using RuntimeDebugger.Logs to your scripts.
	2. Write: LogHandler.AddLog(string DirectoryName, 
		string documentName, string fileExtension, string header);

The LogHandler class will create a new Directory located in 
	Assets/StreamingAssets. The documentName will be the file name.
		The fileExtension will be the file extension and the header
		will display text at the top of the file.