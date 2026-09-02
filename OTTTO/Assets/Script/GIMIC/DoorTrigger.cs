using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public PasswordDoorController doorController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            doorController.ShowUI();
        }
    }
}
