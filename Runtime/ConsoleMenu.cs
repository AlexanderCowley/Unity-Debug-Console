using UnityEngine;
using RuntimeDebugger.Commands;
#if !UNITY_EDITOR
using RuntimeDebugger.Log;
#endif

namespace RuntimeDebugger.Console
{
    /// <summary>
    /// ConsoleMenu handles drawing the Runtime Debugger console
    /// and passing in input to the Command Manager to handle commands
    /// </summary>
    public class ConsoleMenu : MonoBehaviour
    {
        //Console Activation var
        bool _consoleToggle = false;

        //Resolution vars
        float _nativeWidth = 1920;
        float _nativeHeight = 1080;
        float _adjustedWidth = 0;
        
        //Command vars
        string _input;
        public string Message { get; set; } = string.Empty;

        //Singleton Instance vars
        static bool _shutDown = false;
        public static ConsoleMenu _instance;
        public static ConsoleMenu MenuInstance
        {
            get
            {
                if (_shutDown)
                {
                    Debug.LogWarning("Console Menu Instance already destroyed. " +
                        "Returning null.");
                    return null;
                }

                if(_instance == null)
                {
                    _instance = FindObjectOfType<ConsoleMenu>();

                    //In case FindObjectOfType does not find it
                    if(_instance == null)
                    {
                        var menuObject = new GameObject();
                        _instance = menuObject.AddComponent<ConsoleMenu>();
                        menuObject.name = _instance.ToString() + " (Singleton)";
                        DontDestroyOnLoad(menuObject);
                    }
                }
                return _instance;
            }
        }
        
        //Initalizes default commands objects
        void OnEnable() => UtilityCommands.AddDefaultCommands();

        void OnGUI()
        {
            if (!_consoleToggle)
                return;
            CheckConsoleInput();
            UpdateResolution();

            using (var horizontal = new GUILayout.HorizontalScope())
            {
                //Draws console and text field
                DrawConsole();
            }

            //Draws the message using a command
            DrawCommandMessage();
            //Resets the default GUI Color
            GUI.color = Color.white;
        }

        //Checks updates based on GUI Interactions
        void CheckConsoleInput()
        {
            if (Event.current.Equals(Event.KeyboardEvent("return")))
                CheckInput();

            if (Event.current.Equals(Event.KeyboardEvent("backquote")))
                _consoleToggle = !_consoleToggle;
        }

        //Enters input from textfield to command manager then logs and clears input
        void CheckInput()
        {
            CommandManager.ParseCommand(_input);

#if !UNITY_EDITOR
            ConsoleLogger.Log(_input);
#endif

            Message = CommandManager.LastCommand?.Description;
            _input = "";
        }

        //Updates the resolution to scale the console size to match
        void UpdateResolution()
        {
            float rx = Screen.width / _nativeWidth;
            float ry = Screen.height / _nativeHeight;
            // Scale width the same as height - cut off edges to keep ratio the same
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), 
            Quaternion.identity, new Vector3(ry, ry, 1));
            // Get width taking into account edges being cut off or extended
            _adjustedWidth = _nativeWidth * (rx / ry);
        }

        //Draws the console, background color and handles the textfield
        void DrawConsole()
        {
            //Draw Console
            GUI.Box(new Rect(0, 0, _adjustedWidth / 4, _nativeHeight / 22), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            HandleTextfield();
        }

        //Handles the creation, focus of the textfield and assigns input
        void HandleTextfield()
        {                
            //Focus on textfield
            GUI.SetNextControlName("ConsoleInput");
            GUI.skin.textField.fontSize = (int)_nativeWidth / 60;

            //Assigns input
            _input = GUI.TextField(new Rect(0, 0, _adjustedWidth / 4, _nativeHeight / 22), _input);
            GUI.FocusControl("ConsoleInput");
        }

        //Draws message specified in the command object called
        void DrawCommandMessage()
        {
            GUI.color = Color.black;
            GUI.skin.label.fontSize = (int)_nativeWidth / 45;
            GUI.Label(new Rect(5, _nativeHeight - _nativeHeight/10, _adjustedWidth / 4,
            _nativeHeight / 22), Message);
            GUI.color = Color.white;
        }

        void Update()
        {
            //Handles updating input outside of GUI
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _consoleToggle = !_consoleToggle;
        }

        //Handles removing static instance of ConsoleMenu
        void OnDestroy() => _shutDown = true;

        void OnApplicationQuit() => _shutDown = true;
    }
}
