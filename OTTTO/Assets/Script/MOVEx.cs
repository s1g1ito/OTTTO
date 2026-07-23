using UnityEngine;

public class MOVEx : MonoBehaviour
{
    public float speed = 2f;

    private int direction = 1;

    void Update()
    {
        // ˆÚ“®
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // X‚ª2ˆÈã‚È‚ç¶‚Ö
        if (transform.position.x >= 2f)
        {
            direction = -1;
        }

        // X‚ª-2ˆÈ‰º‚È‚ç‰E‚Ö
        if (transform.position.x <= -2f)
        {
            direction = 1;
        }
    }
}