using UnityEngine;

public class PressureTeleport : MonoBehaviour
{
    public Transform teleportTarget; // ƒ[ƒvæ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ­‚µã‚É‚¸‚ç‚µ‚Ä–„‚Ü‚è–h~
            Vector3 pos = teleportTarget.position + Vector3.up * 1f;

            other.transform.position = pos;
        }
    }
}