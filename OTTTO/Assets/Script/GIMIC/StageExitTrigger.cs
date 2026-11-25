using UnityEngine;
using UnityEngine.SceneManagement;

public class StageExitTrigger : MonoBehaviour
{
    [Header("次のステージ")]
    [SerializeField] private string nextSceneName = "Stage2";

    [Header("ゴールの扉")]
    [SerializeField] private DoorMove door;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        CheckExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckExit(other);
    }

    private void CheckExit(Collider other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 扉が完全に開いていなければクリア不可
        if (door == null || !door.IsOpen)
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                nextSceneName +
                "がBuild Profilesに登録されていません");
            return;
        }

        isTransitioning = true;

        Debug.Log("ステージクリア！");

        SceneManager.LoadScene(nextSceneName);
    }
}