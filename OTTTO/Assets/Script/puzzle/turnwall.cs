
using UnityEngine;

public class RotatingDoor : MonoBehaviour
{
    [Header("‰ñ“]Ý’è")]
    [SerializeField] private float speed = 90f;
    [SerializeField] private float angle = 90f;

    private Quaternion startRotation;
    private float currentAngle = 0f;
    private int direction = 1;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        currentAngle += speed * direction * Time.deltaTime;

        if (currentAngle >= angle)
        {
            currentAngle = angle;
            direction = -1;
        }
        else if (currentAngle <= -angle)
        {
            currentAngle = -angle;
            direction = 1;
        }

        transform.localRotation = startRotation *
            Quaternion.Euler(0f, currentAngle, 0f);
    }
}
