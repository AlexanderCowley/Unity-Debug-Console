using UnityEngine;
using TMPro;
public class SetTextTest : MonoBehaviour
{
    TextMeshProUGUI _text;

    void OnEnable()
    {
        CommandManager.AddCommand("scream", "changes text to scream", Scream);
        CommandManager.AddCommand<int>("inttest", "test numbers", IntToText);
        CommandManager.AddCommand<string>("settext", "test strings", SetTextTo);
    }

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.SetText("Hi");
    }

    void Scream()
    {
        _text.SetText("AHHHHHHHHH");
    }

    void IntToText(int number)
    {
        _text.SetText(number.ToString());
    }

    void SetTextTo(string message)
    {
        _text.SetText(message);
    }
}
