using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Stage")]
    public string sceneName;

    // ƒ{ƒ^ƒ“‚ğ‰Ÿ‚µ‚½‚Æ‚«‚ÉŒÄ‚Ño‚·
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}