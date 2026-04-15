using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float checkDistance = 1.5f;
    public float rayHeight = 1.0f;

    private Rigidbody rb;

    private Vector3[] dirs = {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left
    };

    private Vector3 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveDir = dirs[Random.Range(0, 4)];
    }

    void FixedUpdate()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayHeight;

        // デバッグ可視化（4方向）
        Debug.DrawRay(rayOrigin, moveDir * checkDistance, Color.red);
        Debug.DrawRay(rayOrigin, Vector3.right * checkDistance, Color.blue);
        Debug.DrawRay(rayOrigin, Vector3.left * checkDistance, Color.green);

        // 前が壁なら方向転換
        if (Physics.Raycast(rayOrigin, moveDir, checkDistance))
        {
            Turn(rayOrigin);
        }

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    void Turn(Vector3 origin)
    {
        List<Vector3> valid = new List<Vector3>();

        foreach (var d in dirs)
        {
            if (!Physics.Raycast(origin, d, checkDistance))
            {
                valid.Add(d);
            }
        }

        if (valid.Count == 0)
        {
            moveDir = -moveDir;
            return;
        }

        moveDir = valid[Random.Range(0, valid.Count)];
    }
}
