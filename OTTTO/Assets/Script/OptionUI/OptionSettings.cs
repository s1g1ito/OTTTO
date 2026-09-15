using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionSettings : MonoBehaviour
{
    // 画面サイズDropdown
    public TMP_Dropdown screenSizeDropdown;

    // ウィンドウ化Toggle
    public Toggle windowModeToggle;

    // 画面サイズ
    private int[] widths = { 1280, 1600, 1920 };
    private int[] heights = { 720, 900, 1080 };


    void Start()
    {
        // 現在の画面サイズに合わせる
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

        // ウィンドウモードの初期状態
        windowModeToggle.isOn = !Screen.fullScreen;

        // ウィンドウモード変更
        windowModeToggle.onValueChanged.AddListener(ChangeWindowMode);
    }


    // 画面サイズ変更
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


    // ウィンドウモード変更
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
}