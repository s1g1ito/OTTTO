using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [InspectorName("飛ぶ先のシーン")]
    public Object targetScene;

    public void ChangeScene()
    {
        if (targetScene != null)
        {
            SceneManager.LoadScene(targetScene.name);
        }
        else
        {
            Debug.LogWarning("飛ぶ先のシーンが設定されていません");
        }
    }
}