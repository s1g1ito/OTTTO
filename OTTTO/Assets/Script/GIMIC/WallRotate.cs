using UnityEngine;

public class WallRotate : MonoBehaviour
{
    [SerializeField] private float rotateDuration = 3f;

    private bool isRotating = false;
    private float rotateTimer = 0f;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRotating)
        {
            // ‰ñ“]ŠJŽnŽž‚ÌŠp“x‚ð•Û‘¶
            startRotation = transform.rotation;

            // Œ»Ý‚ÌŠp“x‚©‚çYŽ²•ûŒü‚É180“x‰ñ“]‚µ‚½Šp“x‚ðì‚é
            targetRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

            // ƒ^ƒCƒ}[‚ðƒŠƒZƒbƒg
            rotateTimer = 0f;
            isRotating = true;
        }
    }

    private void Update()
    {
        if (isRotating)
        {
            // 0`1‚Ìis“x‚ðŒvŽZ
            rotateTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(rotateTimer / rotateDuration);

            // 3•b‚©‚¯‚Ä180“x‰ñ“]
            transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                progress
            );

            // 3•bŒo‰ß
            if (progress >= 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }
}



