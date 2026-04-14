using UnityEngine;

public class HideItemOnTouch : MonoBehaviour
{
    public string hiddenLayerName = "ItemHidden";

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // G‚ê‚½uŠÔ‚¾‚¯ƒŒƒCƒ„[‚ğ•ÏX
            gameObject.layer = LayerMask.NameToLayer(hiddenLayerName);
        }
    }
}
