using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleBackButton : MonoBehaviour
{
    // Sceneアセット
#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset targetScene;
#endif

    // Scene名を保存
    [SerializeField, HideInInspector]
    private string targetSceneName;

#if UNITY_EDITOR
    // InspectorでSceneを変更したときにScene名を取得
    private void OnValidate()
    {
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
        else
        {
            targetSceneName = "";
        }
    }
#endif

    // ボタンから呼び出す
    public void Back()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("戻り先のSceneが設定されていません！");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}