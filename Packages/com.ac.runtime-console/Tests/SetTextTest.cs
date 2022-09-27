using UnityEngine;
using UnityEngine.UI;
using RuntimeDebugger.Commands;
public class SetTextTest : MonoBehaviour
{
    Text _text;

    void OnEnable()
    {
        CommandManager.AddCommand("scream", "changes text to scream", Scream);
        CommandManager.AddCommand<int>("inttest", "test numbers", IntToText);
        CommandManager.AddCommand<string>("settext", "test strings", SetTextTo);
    }

    void Awake()
    {
        _text = GetComponent<Text>();
        _text.text = "Hi";
    }

    void Scream()
    {
        _text.text = "AHHHHHHHHH";
    }

    void IntToText(int number)
    {
        _text.text = number.ToString();
    }

    void SetTextTo(string message)
    {
        _text.text = message;
    }
}
