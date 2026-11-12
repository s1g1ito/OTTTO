
using UnityEngine;

public class BallChase : MonoBehaviour
{
    public Transform player;

    public float downSpeed = 3f;
    public float chaseSpeed = 5f;
    public float targetHeight = 1f;

    private bool activated = false;
    private bool chasing = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!activated) return;

        if (!chasing)
        {
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

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                chasing = true;
                rb.useGravity = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!chasing) return;

        Vector3 direction = player.position - rb.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Vector3 movement = direction.normalized * chaseSpeed;

            rb.AddForce(movement, ForceMode.Force);
        }
    }

    public void Activate()
    {
        activated = true;
    }
}
