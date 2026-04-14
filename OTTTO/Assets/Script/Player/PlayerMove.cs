using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 0.03f;
    public float rotateSpeed = 2f;

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.upArrowKey.isPressed)
        {
            transform.position += transform.forward * moveSpeed;
        }

        if (keyboard.downArrowKey.isPressed)
        {
            transform.position -= transform.forward * moveSpeed;
        }

        if (keyboard.rightArrowKey.isPressed)
        {
            transform.Rotate(0, rotateSpeed, 0);
        }

        if (keyboard.leftArrowKey.isPressed)
        {
            transform.Rotate(0, -rotateSpeed, 0);
        }
    }
}
