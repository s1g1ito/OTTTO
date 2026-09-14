using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    // スタート地点
    public Transform startPoint;

    // 落下したと判断する高さ
    public float fallHeight = -5f;

    void Update()
    {
        // プレイヤーが一定の高さより下に落ちたら
        if (transform.position.y < fallHeight)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // スタート地点に戻す
        transform.position = startPoint.position;

        // Rigidbodyの速度を止める
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
