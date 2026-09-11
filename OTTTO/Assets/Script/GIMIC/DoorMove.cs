using System.Collections;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    // 右に移動する距離
    public float moveDistance = 5f;

    // 移動にかける時間
    public float moveTime = 2f;

    // Doorの最初の位置
    private Vector3 startPosition;

    // 移動先
    private Vector3 targetPosition;

    // 現在動いているコルーチン
    private Coroutine moveCoroutine;

    void Start()
    {
        // ゲーム開始時の位置を保存
        startPosition = transform.position;

        // 右に5移動した場所
        targetPosition = startPosition + Vector3.right * moveDistance;
    }

    // Doorを右にスライド
    public void MoveRight()
    {
        // すでに動いていたら止める
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 右へ移動
        moveCoroutine = StartCoroutine(MoveDoor(targetPosition));
    }

    // Doorを元の位置に戻す
    public void MoveBack()
    {
        // すでに動いていたら止める
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // 元の位置へ移動
        moveCoroutine = StartCoroutine(MoveDoor(startPosition));
    }

    // Doorを滑らかに移動させる
    IEnumerator MoveDoor(Vector3 target)
    {
        Vector3 start = transform.position;

        float time = 0f;

        while (time < moveTime)
        {
            time += Time.deltaTime;

            float t = time / moveTime;

            transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        // 最後は確実に目的地にする
        transform.position = target;

        moveCoroutine = null;
    }
}