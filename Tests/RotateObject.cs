using UnityEngine;
using RuntimeDebugger.Commands;

namespace RuntimeDebugger.Tests
{
    public class RotateObject : MonoBehaviour
    {
        Transform _objTransform;
        bool _isRotating = false;
        void Awake()
        {
            _objTransform = transform;
            //CommandManager.AddCommand<bool>("rotate", "rotates a gameobject", ToggleRotation, this);
        }
        void Rotate() => _objTransform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.Self);
        void Update()
        {
            if(_isRotating)
                Rotate();
        } 
        [AddCommand("rotate-object")]
        void ToggleRotation(bool rotate)
        {
            _isRotating = !_isRotating;
        }
    }
}
