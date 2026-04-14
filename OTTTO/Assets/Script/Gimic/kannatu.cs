using UnityEngine;

public class kannatu : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Player ‚ª“¥‚ñ‚¾‚¾‚¯”½‰‚³‚¹‚½‚¢ê‡
        if (other.CompareTag("Player"))
        {
            Debug.Log("“¥‚İ‚Ü‚µ‚½I");
        }
    }
}
