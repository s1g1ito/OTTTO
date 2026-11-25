using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSceneChange : MonoBehaviour
{
    [Header("移動先シーン設定")]
    [InspectorName("飛ぶ先のシーン名")]
    [SerializeField] private string targetSceneName = "puzzle";

    [Header("プレイヤー設定")]
    [InspectorName("保存するプレイヤー")]
    public Transform player;

    // プレイヤーの保存用座標
    private Vector3 lastPlayerPosition;
    private Quaternion lastPlayerRotation;

    // シーン移動の重複防止
    private bool isTransitioning = false;

    private void Start()
    {
        // Playerが未設定なら自動取得
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // プレイヤーの初期位置を保存
        if (player != null)
        {
            lastPlayerPosition = player.position;
            lastPlayerRotation = player.rotation;
        }
        else
        {
            Debug.LogWarning(
                "SaveSceneChange：Playerが設定されていません");
        }
    }

    private void Update()
    {
        // プレイヤーの位置を記録
        if (player != null)
        {
            lastPlayerPosition = player.position;
            lastPlayerRotation = player.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 接触したオブジェクトを確認
        Debug.Log(
            "パズル入口に接触：" +
            other.gameObject.name +
            " / Tag：" + other.tag
        );

        // Playerタグを確認
        bool isPlayer = other.CompareTag("Player");

        // 子Colliderの場合も判定する
        if (!isPlayer && other.attachedRigidbody != null)
        {
            isPlayer =
                other.attachedRigidbody.CompareTag("Player");
        }

        // 登録されたPlayerの子Colliderにも対応
        if (!isPlayer && player != null)
        {
            isPlayer =
                other.transform == player ||
                other.transform.IsChildOf(player);
        }

        if (!isPlayer)
        {
            return;
        }

        // 重複してシーン移動しない
        if (isTransitioning)
        {
            return;
        }

        Debug.Log("パズル入口に触れました！");

        // シーン名の確認
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError(
                "飛ぶ先のシーン名が設定されていません");
            return;
        }

        // Build Profilesの登録確認
        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                "シーン「" + targetSceneName +
                "」がBuild Profilesに登録されていません");
            return;
        }

        // Playerが未設定なら接触元から取得
        if (player == null)
        {
            player = other.attachedRigidbody != null
                ? other.attachedRigidbody.transform
                : other.transform;

            lastPlayerPosition = player.position;
            lastPlayerRotation = player.rotation;
        }

        // 戻り位置を保存
        SavePlayerPosition();

        // パズルシーンに移動
        ChangeScene();
    }

    // プレイヤーの位置を保存
    private void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat(
            "Scene5_PlayerPosX",
            lastPlayerPosition.x
        );

        PlayerPrefs.SetFloat(
            "Scene5_PlayerPosY",
            lastPlayerPosition.y
        );

        PlayerPrefs.SetFloat(
            "Scene5_PlayerPosZ",
            lastPlayerPosition.z
        );

        PlayerPrefs.SetFloat(
            "Scene5_PlayerRotY",
            lastPlayerRotation.eulerAngles.y
        );

        // パズルから戻るためのフラグ
        PlayerPrefs.SetInt("ReturnFromPuzzle", 1);

        PlayerPrefs.Save();

        Debug.Log(
            "パズル直前の位置を保存：" +
            lastPlayerPosition
        );
    }

    // パズルシーンへ移動
    private void ChangeScene()
    {
        isTransitioning = true;

        Debug.Log(
            "パズルシーンへ移動します：" +
            targetSceneName
        );

        // パズルではカーソルを使用
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        SceneManager.LoadScene(
            targetSceneName,
            LoadSceneMode.Single
        );
    }
}