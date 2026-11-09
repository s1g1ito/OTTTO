
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    [SerializeField] private Transform player;

    [Header("カメラの高さ")]
    [SerializeField] private float height = 20f;

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        // プレイヤーの真上にカメラを配置
        transform.position = new Vector3(
            player.position.x,
            player.position.y + height,
            player.position.z
        );

        // 常に真下を向く
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}

