
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    [Header("ˆÚ“®Ý’è")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float distance = 5f;

    [Header("ˆÚ“®•ûŒü")]
    [SerializeField] private bool moveHorizontal = true;
    [SerializeField] private bool moveVertical = false;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        if (moveHorizontal)
            moveDirection += Vector3.right;

        if (moveVertical)
            moveDirection += Vector3.up;

        transform.Translate(moveDirection.normalized * direction * speed * Time.deltaTime);

        float movedDistance = Vector3.Distance(
            startPosition,
            transform.position
        );

        if (Vector3.Dot(transform.position - startPosition, moveDirection.normalized) >= distance)
        {
            direction = -1;
        }
        else if (Vector3.Dot(transform.position - startPosition, moveDirection.normalized) <= -distance)
        {
            direction = 1;
        }
    }
}
