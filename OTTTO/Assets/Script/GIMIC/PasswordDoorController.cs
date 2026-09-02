using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PasswordDoorController : MonoBehaviour
{
    private string inputCode = "";
    public Animator doorAnimator;

    public TMP_Text inputText;
    public GameObject uiPanel;

    private void Start()
    {
        uiPanel.SetActive(false);
    }

    public void ShowUI()
    {
        uiPanel.SetActive(true);
        ClearCode();
    }

    public void EnterNumber(string n)
    {
        if (inputCode.Length >= 4) return;

        inputCode += n;
        inputText.text = inputCode;

        if(inputCode.Length == 4)
        {
            CheckCode();
        }
    }

    public void ClearCode()
    {
        inputCode = "";
        inputText.text = "";
    }

    void CheckCode()
    {
        if(inputCode == CodeGenerator.GeneratedCode)
        {
            uiPanel.SetActive(false);
            doorAnimator.SetTrigger("Open");
            StartCoroutine(stage4());
        }
        else 
        {
            ClearCode();
        }
    }

    private System.Collections.IEnumerator stage4()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("stage4");
    }
}
