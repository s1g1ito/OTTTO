using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    public BallMover ballMover;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ballMover.StartMove();
        }
    }
}
