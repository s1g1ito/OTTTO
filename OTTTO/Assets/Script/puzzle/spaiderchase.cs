
using UnityEngine;

public class spaiderchase : MonoBehaviour
{
    public Transform player;

    public float downSpeed = 3f;
    public float chaseSpeed = 5f;
    public float targetHeight = 1f;

    private bool activated = false;
    private bool chasing = false;

    private Rigidbody rb;

    private float rotationSpeed = 360f;
    private float currentRotation = 0f;

    private Quaternion startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        startRotation = transform.rotation;

        // 物理演算による回転を防止
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!activated || chasing) return;

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            targetHeight,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            downSpeed * Time.deltaTime
        );

        // 落下中に180度回転
        currentRotation = Mathf.MoveTowards(
            currentRotation,
            180f,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = startRotation *
            Quaternion.Euler(0f, 0f, currentRotation);

        // 着地判定
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            chasing = true;

            transform.rotation = startRotation *
                Quaternion.Euler(0f, 0f, 180f);

            rb.useGravity = true;

            // 着地時の姿勢を維持
            rb.rotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (!chasing) return;

        if (player == null) return;

        // プレイヤーの方向を取得
        Vector3 direction = player.position - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            // プレイヤーの方向に顔を向ける
            Quaternion targetRotation = Quaternion.LookRotation(-direction);

            // 回転を適用
            rb.MoveRotation(targetRotation);

            // プレイヤーに向かって移動
            Vector3 movement = direction * chaseSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + movement);
        }
    }

    public void Activate()
    {
        activated = true;
    }
}
