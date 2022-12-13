using UnityEngine;
using RuntimeDebugger.Commands;

[RequireComponent(typeof(Light))]
public class CustomizeLight : MonoBehaviour
{
    Light _light;
    float _defaultLightIntensity = 10;
    void Awake()
    {
        _light = GetComponent<Light>();
        CommandManager.AddCommand<bool>("toggle-light", "turns light off or on", ToggleLight);
        CommandManager.AddCommand<float>("light-int", "Changes light intensity", ChangeIntensity);
    }

    void ToggleLight(bool lightValue)
    {
        _light.intensity = lightValue ? _defaultLightIntensity : 0;
    }

    void ChangeIntensity(float intensityValue)
    {
        _light.intensity = intensityValue;
        _defaultLightIntensity = intensityValue;
    }
}
