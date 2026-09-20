using UnityEngine;
using UnityEngine.SceneManagement;

public class CodeSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("CodeScene");
        }
    }
}
