using UnityEngine;

public class BallEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
