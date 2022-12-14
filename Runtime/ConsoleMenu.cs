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

        //Console GUI
        Rect _consoleRect;
        float consoleWidthScalar = 6.8f;
        float _viewCount = 15f;
        Vector2 ScrollPos = Vector2.zero;

        //Declares GUIStyle for console log items
        GUIStyle _logStyle = new GUIStyle();
        GUIContent _logContent = new GUIContent();

        float _labelHeaderOffset = 12f;

        //Resolution vars
        float _nativeWidth = 1920;
        float _nativeHeight = 1080;
        float _adjustedWidth = 0;
        
        //Command vars
        string _input;
        public string Message { get; set; } = string.Empty;

        //Singleton Instance vars
        static bool _shutDown = false;
        static ConsoleMenu _instance;
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
        void OnEnable()
        {
            UtilityCommands.AddDefaultCommands();

            //Console Log Item Style Settings
            _logStyle.wordWrap = true;
            _logStyle.normal.textColor = Color.white;
        } 

        void OnGUI()
        {
            if (!_consoleToggle)
                return;
            CheckConsoleInput();
            UpdateResolution();

            using (var horizontal = new GUILayout.HorizontalScope())
            {
                using(var vertical = new GUILayout.VerticalScope())
                {
                    //Draws the console log
                    DrawConsoleLog();
                }
                //Draws console and text field
                DrawConsole();
            }

            //Draws the description of the last command called
            DrawCommandMessage();
            //Resets the default GUI Color
            GUI.color = Color.white;
        }

        //Checks updates based on GUI Interactions
        void CheckConsoleInput()
        {
            //Checks the enter key to enter in current input
            if (Event.current.Equals(Event.KeyboardEvent("return")))
                CheckInput();

            //Toggles console's active state
            if (Event.current.Equals(Event.KeyboardEvent("backquote")))
                _consoleToggle = !_consoleToggle;
        }

        //Enters input from textfield to command manager then logs and clears input
        void CheckInput()
        {
            //Adds input to log
            CommandManager.InputCommandLogs.Add(_input);
            //parses input for command manager to handle
            CommandManager.ParseCommand(_input);
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
            _consoleRect = new Rect(0, _adjustedWidth / consoleWidthScalar,
            _adjustedWidth / 3, _nativeHeight / 22);
            GUI.Box(_consoleRect, "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            HandleTextfield();
        }

        void DrawConsoleLog()
        {
            int logCount = CommandManager.InputCommandLogs.Count;
            Rect consoleLogRect = new Rect(0, 0, _adjustedWidth / 3, _nativeHeight / 4);
            GUI.Box(consoleLogRect, "");
            
            Rect scrollViewRect = new Rect(3, 6,
            _adjustedWidth / 3, _nativeHeight / 4.2f);
            
            Rect viewRect = new Rect(4, 4, _adjustedWidth / 26, 
            (_nativeHeight / 10) * logCount);
            
            ScrollPos = GUI.BeginScrollView(consoleLogRect, ScrollPos, viewRect);
            
            _logStyle.fontSize = (int)_nativeWidth / 62;
            //calculates scroll position by dividing the scroll position by the font size
            int firstIndex = (int)ScrollPos.y / _logStyle.fontSize;
            Rect labelRect = new Rect(24, (firstIndex * 18f) + (viewRect.y + _labelHeaderOffset), 
            _adjustedWidth / 3, _nativeHeight / 22);
            GUI.skin.label.wordWrap = true;
            GUI.skin.label.fontSize = (int)_nativeWidth / 62;
            //Draw each input element
            for(int i = firstIndex; i < Mathf.Min
            (logCount, firstIndex + _viewCount); i++)
            {
                GUI.Label(labelRect, CommandManager.InputCommandLogs[i]);
                labelRect.y += labelRect.height + 6;
            }
            GUI.EndScrollView();
        }

        //Handles the creation, focus of the textfield and assigns input
        void HandleTextfield()
        {                
            //Focus on textfield
            GUI.SetNextControlName("ConsoleInput");
            GUI.skin.textField.fontSize = (int)_nativeWidth / 60;

            //Assigns input
            _input = GUI.TextField(new Rect(0, _adjustedWidth / consoleWidthScalar,
            _adjustedWidth / 3, _nativeHeight / 8), _input);
            GUI.FocusControl("ConsoleInput");
        }

        //Draws message specified in the command object called
        void DrawCommandMessage()
        {
            GUI.color = Color.black;
            GUI.skin.label.fontSize = (int)_nativeWidth / 45;
            GUI.Label(new Rect(5, _nativeHeight - _nativeHeight/10, _adjustedWidth / 4,
            _nativeHeight / 20), Message);
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
