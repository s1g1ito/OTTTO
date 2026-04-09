using UnityEngine;

public class TouchObject : MonoBehaviour
{
    public GameObject Player;  
    public GameObject target;     

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // ˆÚ“®
            transform.position = new Vector3(
                target.transform.position.x,
                0.5f,
                target.transform.position.z
            );

            // e‚ğ•ÏX
            transform.SetParent(Player.transform);
        }
    }
}
