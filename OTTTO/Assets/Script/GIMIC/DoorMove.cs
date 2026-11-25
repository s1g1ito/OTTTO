using System.Collections;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    [Header("扉の移動設定")]
    public float moveDistance = 5f;
    public float moveTime = 2f;

    // 扉の閉じた位置
    private Vector3 closedPosition;

    // 扉の開いた位置
    private Vector3 openedPosition;

    // 初期ローカル座標
    private Vector3 initialLocalPosition;

    // 現在動作中のコルーチン
    private Coroutine moveCoroutine;

    private bool initialized = false;
    private bool isOpen = false;
    private bool hasInitialLocalPosition = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        SaveInitialLocalPosition();
    }

    private void Start()
    {
        if (!initialized)
        {
            InitializeDoor();
        }
    }

    // 初期ローカル座標を保存
    private void SaveInitialLocalPosition()
    {
        if (hasInitialLocalPosition)
        {
            return;
        }

        initialLocalPosition = transform.localPosition;
        hasInitialLocalPosition = true;
    }

    // 扉の初期位置を設定
    public void InitializeDoor(
        bool restoreLocalPosition = false)
    {
        SaveInitialLocalPosition();

        StopDoorMovement();

        // 子オブジェクトの場合、初期位置に戻す
        if (restoreLocalPosition)
        {
            transform.localPosition = initialLocalPosition;
        }

        // 現在位置を閉じた位置として保存
        closedPosition = transform.position;

        // 扉自身の右方向を開く方向にする
        openedPosition =
            closedPosition + transform.right * moveDistance;

        initialized = true;
        isOpen = false;
    }

    // 扉を開く
    public void MoveRight()
    {
        if (!initialized)
        {
            InitializeDoor();
        }

        StopDoorMovement();

        // Doorが無効な場合は移動させない
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning(
                "Doorが非アクティブなので開けません",
                this
            );

            return;
        }

        isOpen = false;

        moveCoroutine = StartCoroutine(
            MoveDoor(openedPosition, true)
        );
    }

    // 扉を閉じる
    public void MoveBack()
    {
        if (!initialized)
        {
            InitializeDoor();
        }

        StopDoorMovement();

        // Doorが無効ならコルーチンを使わず戻す
        if (!isActiveAndEnabled)
        {
            transform.position = closedPosition;
            isOpen = false;

            return;
        }

        isOpen = false;

        moveCoroutine = StartCoroutine(
            MoveDoor(closedPosition, false)
        );
    }

    // 扉の移動を停止する
    private void StopDoorMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    // 扉を滑らかに移動させる
    private IEnumerator MoveDoor(
        Vector3 target,
        bool willOpen)
    {
        Vector3 start = transform.position;

        // 移動時間が0以下の場合は即座に移動
        if (moveTime <= 0f)
        {
            transform.position = target;
        }
        else
        {
            float time = 0f;

            while (time < moveTime)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(
                    time / moveTime
                );

                transform.position = Vector3.Lerp(
                    start,
                    target,
                    t
                );

                yield return null;
            }

            transform.position = target;
        }

        isOpen = willOpen;
        moveCoroutine = null;
    }
}