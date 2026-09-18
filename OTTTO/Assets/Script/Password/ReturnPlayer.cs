using UnityEngine;

public class ReturnPlayer : MonoBehaviour
{
    public Transform returnPoint;
    public Transform player;

    public void Start()
    {
        if(PlayerPrefs.GetInt("DoorUnlocked", 0) == 1)
        {
            Vector3 safePos = returnPoint.position + Vector3.up * 0.5f;
            player.position = returnPoint.position;
            player.rotation = returnPoint.rotation;
        }
    }
}
