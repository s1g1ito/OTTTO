using UnityEngine;
using UnityEngine.InputSystem;

public class AttachItemToPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, -0.5f, 1f);

    private bool isHeld = false;    // ← 持っているかどうか
    private bool canPick = true;   // ← 拾えるかどうか
    private float pickCooldown = 0.2f; // ← 再取得防止時間

    private void Update()
    {
        var keyboard = Keyboard.current;

        // Qキーで離す
        if (isHeld && keyboard != null && keyboard.qKey.wasPressedThisFrame)
        {
            Drop();
        }

        // 持っている間は Player の前に固定
        if (isHeld)
        {
            FollowPlayer();
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!isHeld && canPick && col.gameObject.CompareTag("Player"))
        {
            Pick(col.transform);
        }
    }

    private void Pick(Transform playerTransform)
    {
        player = playerTransform;
        isHeld = true;

        // Player の前に配置
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;
    }

    private void Drop()
    {
        isHeld = false;

        // 親子関係を解除
        transform.SetParent(null);

        // 一時的に拾えなくする
        canPick = false;
        Invoke(nameof(EnablePick), pickCooldown);
    }

    private void EnablePick()
    {
        canPick = true;
    }

    private void FollowPlayer()
    {
        transform.localPosition = offset;
    }
}
