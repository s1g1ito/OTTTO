using UnityEngine;

public class Enemydestroy : MonoBehaviour
{
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            Destroy(enemy);
        }
    }
}
