using UnityEngine;
using RuntimeDebugger.Commands;
using ConsoleLogger = RuntimeDebugger.Log.ConsoleLogger;

namespace RuntimeDebugger.Console
{
    public class ConsoleMenu : MonoBehaviour
    {
        bool _consoleToggle = false;

        string _input;
        public string Message { get; set; } = string.Empty;

        Resolution screenResolution;
        Matrix4x4 savedMatrix;
        float xScreenSize = 1920;
        float yScreenSize = 1080;
        float GUIRatioX;
        float GUIRatioY;
        Vector3 scaleFactor;

        UtilityCommands initCommands;

        //Figure how to scale GUI

        void OnEnable()
        {
            initCommands = new UtilityCommands();

            GUIRatioX = Screen.width / xScreenSize;
            GUIRatioY = Screen.height / yScreenSize;
            scaleFactor = new Vector3(GUIRatioX, GUIRatioY, 1);
        }

        void OnGUI()
        {
            if (!_consoleToggle)
                return;

            if (Event.current.Equals(Event.KeyboardEvent("return")))
                CheckInput();

            if (Event.current.Equals(Event.KeyboardEvent("backquote")))
                _consoleToggle = !_consoleToggle;

            GUI.matrix = Matrix4x4.TRS(new Vector3(scaleFactor.x, scaleFactor.y, 1),
        Quaternion.identity, scaleFactor);

            GUI.Box(new Rect(0, 0, Screen.width / 4, Screen.height / 22), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);

            GUI.SetNextControlName("ConsoleInput");
            GUI.skin.textField.fontSize = Screen.width / 60;
            _input = GUI.TextField(new Rect(0, 0, Screen.width / 4, Screen.height / 22), _input);
            GUI.FocusControl("ConsoleInput");

            GUI.color = Color.black;
            GUI.Label(new Rect(5, Screen.height - 20, Screen.width / 4, Screen.height / 22), Message);
            GUI.color = Color.white;
        }

        void CheckInput()
        {
            CommandManager.ParseCommand(_input);
            ConsoleLogger.Log(_input);
            Message = CommandManager.LastCommand?.Description;
            _input = "";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _consoleToggle = !_consoleToggle;
        }
    }

}
