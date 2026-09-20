using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    public TMP_InputField inputField;

    public void CheckPassword()
    {
        if(inputField.text == CodeGenerator.GeneratedCode)
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
