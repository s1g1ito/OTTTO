using UnityEngine;
using UnityEngine.SceneManagement;

public class CodeSceneTrigger : MonoBehaviour
{
    [Header("従来ステージ用")]
    public ReturnPosition returnPosition;

    [Header("暗証番号シーン")]
    [SerializeField] private string codeSceneName = "CodeScene";

    [Header("次のステージへ進む設定")]
    [SerializeField] private bool advanceToStage2 = true;
    [SerializeField] private string nextSceneName = "Stage2";

    public static bool AdvanceToStage2 { get; private set; }
    public static string NextSceneName { get; private set; }
    public static string ReturnSceneName { get; private set; }

    private bool isTransitioning = false;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSettings()
    {
        AdvanceToStage2 = false;
        NextSceneName = "Stage2";
        ReturnSceneName = "Stage";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (isTransitioning)
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(codeSceneName))
        {
            Debug.LogError(
                codeSceneName + "がBuild Profilesにありません");
            return;
        }

        // 従来ステージの場合は復帰位置を保存
        if (!advanceToStage2)
        {
            if (returnPosition == null)
            {
                Debug.LogError(
                    "ReturnPositionが未設定です");
                return;
            }

            returnPosition.SetReturnPosition();
        }

        // 暗証番号正解後の移動先を保存
        AdvanceToStage2 = advanceToStage2;
        NextSceneName = nextSceneName;

        ReturnSceneName =
            SceneManager.GetActiveScene().name;

        isTransitioning = true;

        SceneManager.LoadScene(codeSceneName);
    }
}