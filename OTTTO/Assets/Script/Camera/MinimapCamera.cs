
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    [SerializeField] private Transform player;

    [Header("ミニマップ専用カメラ")]
    [SerializeField] private Transform minimapCamera;

    [Header("カメラの高さ")]
    private float height = 13f;

    private void LateUpdate()
    {
        if (player == null || minimapCamera == null)
        {
            return;
        }

        // プレイヤーの位置を取得
        Vector3 playerPos = player.position;

        // 指定したカメラをプレイヤーの真上に移動
        minimapCamera.position = new Vector3(
            playerPos.x,
            playerPos.y + height,
            playerPos.z
        );

        // カメラは常に真下を向く
        minimapCamera.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
