using UnityEngine;

public class Door : MonoBehaviour
{
    public float closeDelay = 2f;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("OpenDoor");
            CancelInvoke(nameof(CloseDoor));
            Invoke(nameof(CloseDoor), closeDelay);
        }
    }

    void CloseDoor()
    {
        animator.SetTrigger("CloseDoor");
    }
}
