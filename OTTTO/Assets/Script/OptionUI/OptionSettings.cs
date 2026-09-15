using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionSettings : MonoBehaviour
{
    // ==============================
    // 画面サイズ
    // ==============================

    // 画面サイズDropdown
    public TMP_Dropdown screenSizeDropdown;

    // 画面サイズ
    private int[] widths = { 1280, 1600, 1920 };
    private int[] heights = { 720, 900, 1080 };


    // ==============================
    // ウィンドウモード
    // ==============================

    // ウィンドウ化Toggle
    public Toggle windowModeToggle;


    // ==============================
    // マウス感度
    // ==============================

    // マウス感度Slider
    public Slider mouseSensitivitySlider;

    // マウス感度を表示するValueText
    public TMP_Text mouseSensitivityText;


    // ==============================
    // Start
    // ==============================

    void Start()
    {
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

        // 画面サイズ変更
        screenSizeDropdown.onValueChanged.AddListener(ChangeScreenSize);


        // ------------------------------
        // ウィンドウモード
        // ------------------------------

        // 現在がウィンドウならON
        windowModeToggle.isOn = !Screen.fullScreen;

        // ウィンドウモード変更
        windowModeToggle.onValueChanged.AddListener(ChangeWindowMode);


        // ------------------------------
        // マウス感度
        // ------------------------------

        // 初期値
        mouseSensitivitySlider.value = 1.0f;

        // Sliderが変更されたら呼ぶ
        mouseSensitivitySlider.onValueChanged.AddListener(ChangeMouseSensitivity);

        // 初期値を表示
        ChangeMouseSensitivity(mouseSensitivitySlider.value);
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
        // 画面に「1.00」のように表示
        mouseSensitivityText.text = value.ToString("F2");

        // マウス感度を保存
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        Debug.Log("マウス感度を保存: " + value);
    }
}