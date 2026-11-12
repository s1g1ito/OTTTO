
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    public GameObject ball;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(ball);
        }
    }
}
