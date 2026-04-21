using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private InputAction moveAction;
    public Transform cameraTransform;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        moveAction = new InputAction(type: InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Player ‚Ì forward/right ‚ðŽg‚Á‚ÄˆÚ“®
        Vector3 move = transform.forward * input.y + transform.right * input.x;

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}
