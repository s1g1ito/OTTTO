using UnityEngine;

public class ReturnPlayer : MonoBehaviour
{
    public Transform returnPoint;
    public Transform player;

    public void Start()
    {
        if(PlayerPrefs.GetInt("DoorUnlocked", 0) == 1)
        {
            player.position = returnPoint.position;
            player.rotation = returnPoint.rotation;
        }
    }
}
