using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public bool isReversed = false; // © ”½“]ƒtƒ‰ƒO

    void Update()
    {
        float input = Input.GetAxis("Horizontal");

        if (isReversed)
        {
            input *= -1; // © ”½“]
        }

        Vector3 move = new Vector3(input * speed * Time.deltaTime, 0, 0);
        transform.Translate(move);
    }
}
