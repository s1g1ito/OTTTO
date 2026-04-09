using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;        // Player の Transform
    public float chaseRange = 10f;  // 追いかけ始める距離
    public float moveSpeed = 3f;    // 移動速度

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 倒れないように固定
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 一定距離以内なら追いかける
        if (distance < chaseRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // 上下方向は無視（地面を走るため）

            rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

            // Player の方向を向く
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
}
