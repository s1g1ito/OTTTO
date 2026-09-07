using UnityEngine;

public class DisableMouseLookTrigger : MonoBehaviour
{
    public MonoBehaviour MouseLookHorizon;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<MouseLookHorizon>().canLook = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<MouseLookHorizon>().canLook = true;
        }
    }
}
