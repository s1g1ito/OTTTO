using UnityEngine;

public class Enemydestroy : MonoBehaviour
{
    public GameObject[] enemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == other.gameObject)
                {
                    Destroy(enemies[i]);
                    break;
                }
            }
        }
    }
}