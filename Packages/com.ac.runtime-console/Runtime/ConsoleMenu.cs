using UnityEngine;
using RuntimeDebugger.Commands;
#if !UNITY_EDITOR
using RuntimeDebugger.Log;
#endif

namespace RuntimeDebugger.Console
{
    public class ConsoleMenu : MonoBehaviour
    {
        bool _consoleToggle = false;
        static bool _shutDown = false;

        string _input;
        public string Message { get; set; } = string.Empty;

        float _nativeWidth = 1920;
        float _nativeHeight = 1080;

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

        void OnEnable() => UtilityCommands.AddDefaultCommands();

        void OnGUI()
        {
            if (!_consoleToggle)
                return;

            if (Event.current.Equals(Event.KeyboardEvent("return")))
                CheckInput();

            if (Event.current.Equals(Event.KeyboardEvent("backquote")))
                _consoleToggle = !_consoleToggle;

            float rx = Screen.width / _nativeWidth;
            float ry = Screen.height / _nativeHeight;
            // Scale width the same as height - cut off edges to keep ratio the same
            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(ry, ry, 1));
            // Get width taking into account edges being cut off or extended
            float adjustedWidth = _nativeWidth * (rx / ry);

            using (var horizontal = new GUILayout.HorizontalScope())
            {
                GUI.Box(new Rect(0, 0, adjustedWidth / 4, _nativeHeight / 22), "");
                GUI.backgroundColor = new Color(0, 0, 0, 0);

                GUI.SetNextControlName("ConsoleInput");
                GUI.skin.textField.fontSize = (int)_nativeWidth / 60;
                _input = GUI.TextField(new Rect(0, 0, adjustedWidth / 4, _nativeHeight / 22), _input);
                GUI.FocusControl("ConsoleInput");
            }

            GUI.color = Color.black;
            GUI.skin.label.fontSize = (int)_nativeWidth / 45;
            GUI.Label(new Rect(5, _nativeHeight - _nativeHeight/10, adjustedWidth / 4, _nativeHeight / 22), Message);
            GUI.color = Color.white;
        }

        void CheckInput()
        {
            CommandManager.ParseCommand(_input);

#if !UNITY_EDITOR
            ConsoleLogger.Log(_input);
#endif

            Message = CommandManager.LastCommand?.Description;
            _input = "";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _consoleToggle = !_consoleToggle;
        }

        void OnDestroy() => _shutDown = true;

        void OnApplicationQuit() => _shutDown = true;
    }

}
