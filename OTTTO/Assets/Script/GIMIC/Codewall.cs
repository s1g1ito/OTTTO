using UnityEngine;

public class RotatingWall : MonoBehaviour
{
    [SerializeField] private float rotateAngle = 90f;
    [SerializeField] private float rotateSpeed = 180f;

    private bool isRotating = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (isRotating)
            return;

        targetRotation *= Quaternion.Euler(0f, rotateAngle, 0f);
        isRotating = true;
    }

    private void Update()
    {
        if (!isRotating)
            return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation;
            isRotating = false;
        }
    }
}
