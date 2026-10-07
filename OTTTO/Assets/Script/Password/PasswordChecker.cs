using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        // マウスカーソルを表示
        Cursor.visible = true;

        // マウスカーソルの固定を解除
        Cursor.lockState = CursorLockMode.None;
    }

    public void CheckPassword()
    {
        if (inputField.text == CodeGenerator.GeneratedCode)
        {
            PlayerPrefs.SetInt("DoorUnlocked", 1);

            SceneManager.LoadScene("Stage");
        }
        else
        {
            Debug.Log("パスワードが違います");
        }
    }
}