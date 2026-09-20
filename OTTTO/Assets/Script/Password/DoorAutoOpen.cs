using UnityEngine;

public class DoorAutoOpen : MonoBehaviour
{
    public Animator doorAnimator;

    public void Start()
    {
        if(PlayerPrefs.GetInt("DoorUnlooked", 0) == 1)
        {
            doorAnimator.SetTrigger("Open");
        }
    }
}
