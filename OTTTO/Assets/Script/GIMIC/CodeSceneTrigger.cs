using UnityEngine;
using UnityEngine.SceneManagement;

public class CodeSceneTrigger : MonoBehaviour
{
    public ReturnPosition returnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            returnPosition.SetReturnPosition();

            SceneManager.LoadScene("CodeScene");
        }
    }
}
