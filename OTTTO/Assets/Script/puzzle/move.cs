using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "puzzle")
        {
            SceneManager.LoadScene("puzzle");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
