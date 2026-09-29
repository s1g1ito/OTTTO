using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSceneChange : MonoBehaviour
{
    [InspectorName("飛ぶ先のシーン")]
    public Object targetScene;

    [InspectorName("保存するプレイヤー")]
    public Transform player;

    // 1フレーム前のPlayer位置
    private Vector3 lastPlayerPosition;
    private Quaternion lastPlayerRotation;

    void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
            lastPlayerRotation = player.rotation;
        }
    }

    void Update()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
            lastPlayerRotation = player.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("パズル入口に触れました");

            // パズル直前の位置を保存
            SavePlayerPosition();

            // パズルシーンへ
            ChangeScene();
        }
    }

    void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat("Scene5_PlayerPosX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("Scene5_PlayerPosY", lastPlayerPosition.y);
        PlayerPrefs.SetFloat("Scene5_PlayerPosZ", lastPlayerPosition.z);

        PlayerPrefs.SetFloat(
            "Scene5_PlayerRotY",
            lastPlayerRotation.eulerAngles.y
        );

        // 「パズルから戻ってきた」という印
        PlayerPrefs.SetInt("ReturnFromPuzzle", 1);

        PlayerPrefs.Save();

        Debug.Log(
            "パズル直前の位置を保存: "
            + lastPlayerPosition
        );
    }

    void ChangeScene()
    {
        if (targetScene != null)
        {
            Debug.Log("パズルシーンへ移動します");

            SceneManager.LoadScene(targetScene.name);

            // パズルではカーソルを使う
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Debug.LogWarning("飛ぶ先のシーンが設定されていません");
        }
    }
}