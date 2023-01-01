using UnityEngine;
using RuntimeDebugger.Commands;

namespace RuntimeDebugger.Tests
{
    [RequireComponent(typeof(Light))]
    public class CustomizeLight : MonoBehaviour
    {
        Light _light;
        float _defaultLightIntensity = 10;
        void Awake()
        {
            _light = GetComponent<Light>();
        }
        [AddCommand("light")]
        void ToggleLight(bool lightValue)
        {
            _light.intensity = lightValue ? _defaultLightIntensity : 0;
        }
        [AddCommand("light-int")]
        void ChangeIntensity(float intensityValue)
        {
            _light.intensity = intensityValue;
            _defaultLightIntensity = intensityValue;
        }
    }
}

