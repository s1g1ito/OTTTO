
using UnityEngine;

public class BallMover : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    private Rigidbody rb;

    private bool gravityStarted = false;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 最初は重力を無効にする
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    // 範囲に入ったら重力を付与
    public void StartMove()
    {
        if (gravityStarted) return;

        gravityStarted = true;
        rb.useGravity = true;

        Debug.Log("重力発動！");
    }

    private void FixedUpdate()
    {
        // 着地後にプレイヤーを追いかける
        if (isChasing && player != null)
        {
            Vector3 direction = player.position - rb.position;

            // 上下方向には追いかけない
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                direction.Normalize();

                Vector3 nextPosition =
                    rb.position + direction * followSpeed * Time.fixedDeltaTime;

                rb.MovePosition(nextPosition);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 地面に着地したら追跡開始
        if (gravityStarted &&
            !isChasing &&
            collision.gameObject.CompareTag("Ground"))
        {
            isChasing = true;

            Debug.Log("プレイヤー追跡開始！");
        }
    }
}
