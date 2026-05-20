using UnityEngine;
using UnityEngine.InputSystem;
public class MouseLook : MonoBehaviour 
{ 
    public float sensitivity = 0.2f; private void Update() 
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        transform.Rotate(0, mouseDelta.x * sensitivity, 0);
    }
}