
using UnityEngine;

public class OperationGuide : MonoBehaviour
{
    [Header("操作ガイドの設定")]
    [SerializeField] private GameObject operationGuidePanel;

    [Header("表示・非表示のキー")]
    [SerializeField] private KeyCode toggleKey = KeyCode.H;

    [Header("表示中にゲームを一時停止する")]
    [SerializeField] private bool pauseGameWhileOpen = true;

    private bool isOpen = false;

    // 元のゲーム状態を保存
    private float previousTimeScale;
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;

    void Awake()
    {
        // 最初は操作ガイドを非表示にする
        operationGuidePanel.SetActive(false);
    }

    void Update()
    {
        // Hキーが押されたとき
        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen)
            {
                CloseGuide();
            }
            else
            {
                OpenGuide();
            }
        }
    }

    // 操作ガイドを表示
    public void OpenGuide()
    {
        if (isOpen) return;

        isOpen = true;

        // 現在の状態を保存
        previousTimeScale = Time.timeScale;
        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        // パネルを表示
        operationGuidePanel.SetActive(true);

        // マウスカーソルを表示
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ゲームを一時停止
        if (pauseGameWhileOpen)
        {
            Time.timeScale = 0f;
        }
    }

    // 操作ガイドを閉じる
    public void CloseGuide()
    {
        if (!isOpen) return;

        isOpen = false;

        // パネルを非表示
        operationGuidePanel.SetActive(false);

        // ゲームの時間を元に戻す
        if (pauseGameWhileOpen)
        {
            Time.timeScale = previousTimeScale;
        }

        // マウスカーソルを元の状態に戻す
        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisible;
    }

    // オブジェクトが無効になった場合の復元処理
    void OnDisable()
    {
        if (isOpen)
        {
            CloseGuide();
        }
    }
}

