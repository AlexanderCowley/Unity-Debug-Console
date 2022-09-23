using UnityEngine;

public class ConsoleMenu : MonoBehaviour
{
    bool _consoleToggle = false;

    string _input;
    public string Message { get; set; } = "";

    UtilityCommands initCommands;

    //Figure how to scale GUI

    void OnEnable()
    {
        initCommands = new UtilityCommands();
    }

    void OnGUI()
    {
        if (!_consoleToggle)
            return;

        if (Event.current.Equals(Event.KeyboardEvent("return")))
            CheckInput();

        if (Event.current.Equals(Event.KeyboardEvent("backquote")))
            _consoleToggle = !_consoleToggle;

        GUI.Box(new Rect(0,0, Screen.width - 320f, 20), "");
        GUI.backgroundColor = new Color(0,0,0,0);

        GUI.SetNextControlName("ConsoleInput");
        _input = GUI.TextField(new Rect(0, 0, Screen.width - 320f, 20f), _input);
        GUI.FocusControl("ConsoleInput");

        GUI.color = Color.black;
        GUI.Label(new Rect(5, Screen.height - 20, Screen.width - 20f, 20f), Message);
        GUI.color = Color.white;
    }

    void CheckInput()
    {
        CommandManager.ParseCommand(_input);
        Message = CommandManager.LastCommand?.Description;
        _input = "";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
            _consoleToggle = !_consoleToggle;
    }
}
