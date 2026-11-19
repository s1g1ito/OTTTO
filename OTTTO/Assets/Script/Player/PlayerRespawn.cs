
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("リスポーン設定")]
    public Transform startPoint;
    public float fallHeight = -5f;

    [Header("ミニマップ")]
    [SerializeField] private FogOfWar fogOfWar;

    private void Update()
    {
        // プレイヤーが落下した場合
        if (transform.position.y < fallHeight)
        {
            Respawn();
        }
    }

    // 通常のColliderに接触した場合
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gimick") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // TriggerのColliderに接触した場合
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gimick") ||
            other.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // プレイヤーがやられたときの処理
    public void Respawn()
    {
        if (startPoint == null)
        {
            Debug.LogError("スタート地点が未設定です");
            return;
        }

        // スタート地点に戻す
        transform.position = startPoint.position;

        // Rigidbodyの速度を止める
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.position = startPoint.position;

            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // ミニマップの探索記録を初期化
        if (fogOfWar != null)
        {
            fogOfWar.ResetExploration();
        }
        else
        {
            Debug.LogWarning("FogOfWarが未設定です");
        }

        Debug.Log("プレイヤーがリスポーンしました");
    }
}
