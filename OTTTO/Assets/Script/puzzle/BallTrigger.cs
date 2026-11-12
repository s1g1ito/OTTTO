
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    public BallChase ball;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("‰½‚©‚ª”ÍˆÍ‚É“ü‚è‚Ü‚µ‚½");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player‚ğŒŸ’m‚µ‚Ü‚µ‚½I");
            ball.Activate();
        }
    }
}
