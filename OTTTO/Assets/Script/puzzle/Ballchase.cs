
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

    private float rotationSpeed = 360f;
    private float currentRotation = 0f;



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

            currentRotation = Mathf.MoveTowards(
    currentRotation,
    180f,
    rotationSpeed * Time.deltaTime
);

            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
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
