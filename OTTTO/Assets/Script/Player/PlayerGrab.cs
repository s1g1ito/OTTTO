using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    public Rigidbody handRb;

    private GameObject currentItem;
    private ConfigurableJoint currentJoint;

    void Update()
    {
        // EÉLÅ[Ç≈íÕÇﬁ / ó£Ç∑
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentJoint == null && currentItem != null)
            {
                Grab(currentItem.GetComponent<Rigidbody>());
            }
            else
            {
                Release();
            }
        }
    }

    void Grab(Rigidbody targetRb)
    {
        currentJoint = targetRb.gameObject.AddComponent<ConfigurableJoint>();
        currentJoint.connectedBody = handRb;

        currentJoint.xMotion = ConfigurableJointMotion.Limited;
        currentJoint.yMotion = ConfigurableJointMotion.Limited;
        currentJoint.zMotion = ConfigurableJointMotion.Limited;

        JointDrive drive = new JointDrive
        {
            positionSpring = 3000f,
            positionDamper = 200f,
            maximumForce = Mathf.Infinity
        };

        currentJoint.xDrive = drive;
        currentJoint.yDrive = drive;
        currentJoint.zDrive = drive;
    }

    void Release()
    {
        if (currentJoint != null)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            currentItem = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (currentItem == other.gameObject)
                currentItem = null;
        }
    }
}