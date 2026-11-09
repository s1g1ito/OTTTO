
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToStage : MonoBehaviour
{
    public void ReturnToStage()
    {
        SceneManager.LoadScene("Stage");
    }
}
