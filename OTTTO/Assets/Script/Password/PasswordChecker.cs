using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PasswordChecker : MonoBehaviour
{
    [Header("暗証番号入力")]
    public TMP_InputField inputField;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CheckPassword()
    {
        if (inputField == null)
        {
            Debug.LogError("InputFieldが未設定です");
            return;
        }

        // 暗証番号が不正解の場合
        if (inputField.text != CodeGenerator.GeneratedCode)
        {
            Debug.Log("パスワードが違います");
            return;
        }

        Debug.Log("暗証番号が正解しました！");

        // Stage1から来た場合
        if (CodeSceneTrigger.AdvanceToStage2)
        {
            string nextScene = CodeSceneTrigger.NextSceneName;

            if (!Application.CanStreamedLevelBeLoaded(nextScene))
            {
                Debug.LogError(
                    nextScene + "がBuild Profilesにありません");
                return;
            }

            // Stage2へ移動
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            // 従来の開錠処理
            string returnScene =
                CodeSceneTrigger.ReturnSceneName;

            if (!Application.CanStreamedLevelBeLoaded(returnScene))
            {
                Debug.LogError(
                    returnScene + "がBuild Profilesにありません");
                return;
            }

            PlayerPrefs.SetInt("DoorUnlocked", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(returnScene);
        }
    }
}