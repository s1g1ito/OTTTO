using UnityEngine;
using UnityEngine.InputSystem;

public class EscOpenOption : MonoBehaviour
{
    // オプション画面
    public GameObject optionCanvas;

    void Update()
    {
        // Escキーを押したら
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OpenOption();
        }
    }

    void OpenOption()
    {
        optionCanvas.SetActive(true);
    }
}