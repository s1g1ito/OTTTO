using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "puzzle")
        {
            SceneManager.LoadScene("puzzle");

            Debug.Log("puzzle‚ÉˆÚ“®‚µ‚Ü‚µ‚½");

            this.transform.position = new Vector3(48f, 1f, 44f);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
