using UnityEngine;

public class ConsoleMenu : MonoBehaviour
{
    //Console Menu
    //Static Instance?
    //Input
    //Drawing GUI

    //GUI Logger
    //Logging previous Messages
    //Drawing Messages

    bool _consoleToggle = false;

    string _input;
    string _message = "";

    //float currentScreenWidth;
    //float currentScreenHeight;

    //Figure how to scale GUI
    /*void UpdateScreenResoltion()
    {
        currentScreenWidth = Screen.currentResolution.width;
        currentScreenHeight = Screen.currentResolution.height;
    }*/

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


        GUI.Label(new Rect(5, Screen.height - 20, Screen.width - 20f, 20f), _message);
    }

    void CheckInput()
    {
        _message = "No Command Found";
        _input = "";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
            _consoleToggle = !_consoleToggle;
    }
}
