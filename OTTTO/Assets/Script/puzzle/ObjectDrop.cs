
using UnityEngine;

public class ObjectDrop : MonoBehaviour
{
    public GameObject fallingObject;

    [Header("—‰ºİ’è")]
    public float fallSpeed = 5f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            Rigidbody rb = fallingObject.GetComponent<Rigidbody>();

            // d—Í‚ğ—LŒø‰»
            rb.useGravity = true;

            // —‰º‘¬“x‚ğİ’è
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                -fallSpeed,
                rb.linearVelocity.z
            );

            // 3•bŒã‚Éíœ
            Destroy(fallingObject, 3f);
        }
    }
}
