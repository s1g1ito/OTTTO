using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    // 移動先のシーン
    public Object targetScene;

    // シーンを読み込む
    public void ChangeScene()
    {
        if (targetScene != null)
        {
            SceneManager.LoadScene(targetScene.name);
        }
        else
        {
            Debug.LogWarning("移動先のシーンが設定されていません");
        }
    }
}