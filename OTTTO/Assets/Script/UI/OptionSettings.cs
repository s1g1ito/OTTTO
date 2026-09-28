using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OptionSettings : MonoBehaviour
{
    // ==============================
    // オプション画面
    // ==============================

    // オプション画面のCanvas
    public GameObject optionCanvas;

    // OptionシーンではON
    // StageシーンではOFF
    public bool showOnStart = false;


    // ==============================
    // 画面サイズ
    // ==============================

    public TMP_Dropdown screenSizeDropdown;

    private int[] widths = { 1280, 1600, 1920 };
    private int[] heights = { 720, 900, 1080 };


    // ==============================
    // ウィンドウモード
    // ==============================

    public Toggle windowModeToggle;


    // ==============================
    // マウス感度
    // ==============================

    public Slider mouseSensitivitySlider;

    public TMP_Text mouseSensitivityText;


    // ==============================
    // Start
    // ==============================

    void Start()
    {
        // ------------------------------
        // オプション画面の初期状態
        // ------------------------------

        if (showOnStart)
        {
            // Optionシーン
            optionCanvas.SetActive(true);

            // ゲームを停止
            Time.timeScale = 0f;

            // マウスカーソルを表示
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Stageシーン
            optionCanvas.SetActive(false);

            // ゲームを通常状態にする
            Time.timeScale = 1f;

            // マウスカーソルをゲーム用にする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        // ------------------------------
        // 現在の画面サイズをDropdownに反映
        // ------------------------------

        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        for (int i = 0; i < widths.Length; i++)
        {
            if (currentWidth == widths[i] &&
                currentHeight == heights[i])
            {
                screenSizeDropdown.value = i;
                break;
            }
        }

        screenSizeDropdown.onValueChanged.AddListener(ChangeScreenSize);


        // ------------------------------
        // ウィンドウモード
        // ------------------------------

        // 現在がウィンドウならON
        windowModeToggle.isOn = !Screen.fullScreen;

        windowModeToggle.onValueChanged.AddListener(ChangeWindowMode);


        // ------------------------------
        // マウス感度
        // ------------------------------

        // 保存されているマウス感度を読み込む
        float savedSensitivity =
            PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);

        mouseSensitivitySlider.value = savedSensitivity;

        // 初期値を表示
        ChangeMouseSensitivity(savedSensitivity);

        // Sliderが変更されたら呼ぶ
        mouseSensitivitySlider.onValueChanged.AddListener(
            ChangeMouseSensitivity
        );
    }


    // ==============================
    // Update
    // ==============================

    void Update()
    {
        // OptionシーンではEscで閉じない
        if (showOnStart)
        {
            return;
        }

        // Escキーを押したら
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // オプション画面が表示中なら閉じる
            if (optionCanvas.activeSelf)
            {
                CloseOption();
            }
            // 非表示なら開く
            else
            {
                OpenOption();
            }
        }
    }


    // ==============================
    // オプション画面を開く
    // ==============================

    public void OpenOption()
    {
        optionCanvas.SetActive(true);

        // ゲームを一時停止
        Time.timeScale = 0f;

        // マウスカーソルを表示
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("オプション画面を開きました");
    }


    // ==============================
    // オプション画面を閉じる
    // ==============================

    public void CloseOption()
    {
        optionCanvas.SetActive(false);

        // ゲームを再開
        Time.timeScale = 1f;

        // マウスカーソルをゲーム用に戻す
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("オプション画面を閉じました");
    }


    // ==============================
    // 画面サイズ変更
    // ==============================

    void ChangeScreenSize(int index)
    {
        Screen.SetResolution(
            widths[index],
            heights[index],
            Screen.fullScreenMode
        );

        Debug.Log(
            "画面サイズ変更: " +
            widths[index] + "x" + heights[index]
        );
    }


    // ==============================
    // ウィンドウモード変更
    // ==============================

    void ChangeWindowMode(bool isWindow)
    {
        if (isWindow)
        {
            // ウィンドウモード
            Screen.SetResolution(
                Screen.width,
                Screen.height,
                FullScreenMode.Windowed
            );

            Debug.Log("ウィンドウモード");
        }
        else
        {
            // フルスクリーン
            Screen.SetResolution(
                Screen.width,
                Screen.height,
                FullScreenMode.FullScreenWindow
            );

            Debug.Log("フルスクリーン");
        }
    }


    // ==============================
    // マウス感度変更
    // ==============================

    void ChangeMouseSensitivity(float value)
    {
        // 「1.00」のように表示
        mouseSensitivityText.text = value.ToString("F2");

        // マウス感度を保存
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        Debug.Log("マウス感度を保存: " + value);
    }
}