using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    public TMP_InputField inputField;

    public void CheckPassword()
    {
        if(inputField.text == "1234")
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
