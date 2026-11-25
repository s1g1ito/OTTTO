using System.Collections;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    [Header("扉の移動設定")]
    public float moveDistance = 5f;
    public float moveTime = 2f;

    private Vector3 closedPosition;
    private Vector3 openedPosition;

    // 扉の元々のローカル座標
    private Vector3 initialLocalPosition;

    private Coroutine moveCoroutine;
    private bool initialized = false;
    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    private void Start()
    {
        if (!initialized)
        {
            InitializeDoor();
        }
    }

    // 扉の初期位置を設定する
    public void InitializeDoor(
        bool restoreLocalPosition = false)
    {
        // 移動中なら停止
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        // 親オブジェクトの子になっている場合、
        // 生成前のローカル位置に戻す
        if (restoreLocalPosition)
        {
            transform.localPosition = initialLocalPosition;
        }

        // 今の配置場所を閉じた位置として記憶
        closedPosition = transform.position;

        // 扉自身の右方向に移動する
        openedPosition =
            closedPosition + transform.right * moveDistance;

        initialized = true;
        isOpen = false;
    }

    // 扉を右にスライド
    public void MoveRight()
    {
        if (!initialized)
        {
            InitializeDoor();
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        isOpen = false;

        moveCoroutine = StartCoroutine(
            MoveDoor(openedPosition, true));
    }

    // 扉を元の位置に戻す
    public void MoveBack()
    {
        if (!initialized)
        {
            InitializeDoor();
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        isOpen = false;

        moveCoroutine = StartCoroutine(
            MoveDoor(closedPosition, false));
    }

    // 扉を滑らかに移動させる
    private IEnumerator MoveDoor(
        Vector3 target, bool willOpen)
    {
        Vector3 start = transform.position;

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

                float t = Mathf.Clamp01(time / moveTime);

                transform.position =
                    Vector3.Lerp(start, target, t);

                yield return null;
            }

            transform.position = target;
        }

        isOpen = willOpen;
        moveCoroutine = null;
    }
}