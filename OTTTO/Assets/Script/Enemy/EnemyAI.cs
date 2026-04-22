using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rayHeight = 1.0f;
    public float viewDistance = 10f;   // 視界距離
    public float viewAngle = 90f;      // 視界角度

    private Rigidbody rb;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;

        if (CanSeePlayer(origin))
        {
            ChasePlayer();
        }
    }

    // ★ プレイヤー追跡
    void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0; // 上下のズレを無視

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    // ★ 視界判定（Raycast）
    bool CanSeePlayer(Vector3 origin)
    {
        Vector3 toPlayer = player.position - origin;
        toPlayer.y = 0;

        // 距離チェック
        if (toPlayer.magnitude > viewDistance)
            return false;

        // 視界角度チェック
        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > viewAngle)
            return false;

        // Raycast で遮蔽物チェック
        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, viewDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}
