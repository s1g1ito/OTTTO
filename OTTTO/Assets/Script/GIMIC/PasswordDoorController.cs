using UnityEngine;
using UnityEngine.SceneManagement;

public class PasswordDoorController : MonoBehaviour
{
    private string inputCode = "";
    public Animator doorAnimator;

    public void EnterNumber(string n)
    {
        if (inputCode.Length >= 4) return;

        inputCode += n;

        if(inputCode.Length == 4)
        {
            CheckCode();
        }
    }

    public void ClearCode()
    {
        inputCode = "";
    }

    void CheckCode()
    {
        if(inputCode == CodeGenerator.GeneratedCode)
        {
            doorAnimator.SetTrigger("Open");
            StartCoroutine(stage4());
        }
        else 
        {
            inputCode = "";
        }
    }

    private System.Collections.IEnumerator stage4()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("stage4");
    }
}
