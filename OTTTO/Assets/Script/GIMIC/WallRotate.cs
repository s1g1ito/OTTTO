using UnityEngine;

public class WallRotate : MonoBehaviour
{
    private bool rotated = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (rotated) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            transform.Rotate(0f, 180f, 0f);
            rotated = true;
        }
    }
}
