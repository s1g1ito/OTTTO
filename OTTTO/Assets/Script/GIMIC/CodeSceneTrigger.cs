using UnityEngine;
using UnityEngine.SceneManagement;

public class CodeSceneTrigger : MonoBehaviour
{
    public string codeSceneName = "CodeScene";

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("CodeScene");
        }
    }
}
