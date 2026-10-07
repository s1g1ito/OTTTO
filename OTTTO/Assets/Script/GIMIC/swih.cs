using UnityEngine;

public class StepRotateWall : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private float rotateDuration = 3f;

    private bool rotating = false;
    private float timer = 0f;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !rotating)
        {
            startRotation = wall.transform.rotation;
            targetRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

            timer = 0f;
            rotating = true;
        }
    }

    private void Update()
    {
        if (!rotating)
            return;

        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / rotateDuration);

        wall.transform.rotation = Quaternion.Slerp(
            startRotation,
            targetRotation,
            progress
        );

        if (progress >= 1f)
        {
            wall.transform.rotation = targetRotation;
            rotating = false;
        }
    }
}

