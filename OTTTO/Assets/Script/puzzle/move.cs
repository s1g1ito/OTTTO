using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{
    private float speed = 3.0f;

    void Start()
    {
        DontDestroyOnLoad(gameObject); //ƒV[ƒ“‚ğØ‚è‘Ö‚¦‚Ä‚àíœ‚µ‚È‚¢
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.position = new Vector3(
            transform.position.x + moveX,
            transform.position.y,
            transform.position.z + moveZ
            );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "puzzle")
        {
            SceneManager.LoadScene("puzzle");

            Debug.Log("puzzle‚ÉˆÚ“®‚µ‚Ü‚µ‚½");

            this.transform.position = new Vector3(48f, 1f, 44f);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
