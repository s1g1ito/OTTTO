using UnityEngine;

public class WallRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 180f;

    private bool isRotating = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isRotating = true;
        }
    }

    private void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

            if (transform.eulerAngles.y >= 180f)
            {
                transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    180f,
                    transform.eulerAngles.z
                );

                isRotating = false;
            }
        }
    }
}



