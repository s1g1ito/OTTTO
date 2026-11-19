
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MapDisplayController : MonoBehaviour
{
    [Header("ミニマップ")]
    [SerializeField] private GameObject mapPanel;

    [Header("使用制限")]
    [SerializeField] private int maxUses = 5;
    [SerializeField] private float maxViewTime = 15f;

    [Header("UI表示（任意）")]
    [SerializeField] private TMP_Text usesText;
    [SerializeField] private TMP_Text timerText;

    private int remainingUses;
    private float remainingTime;
    private bool isMapOpen = false;

    private void Start()
    {
        ResetMapUsage();
    }

    private void Update()
    {
        if (Keyboard.current == null || mapPanel == null)
        {
            return;
        }

        // Qキーを押した瞬間
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (remainingUses > 0 && !isMapOpen)
            {
                OpenMap();
            }
        }

        if (isMapOpen)
        {
            // 残り時間を減らす
            remainingTime -= Time.unscaledDeltaTime;

            // 15秒経過、またはQキーを離した場合
            if (remainingTime <= 0f ||
                !Keyboard.current.qKey.isPressed)
            {
                CloseMap();
            }
        }

        UpdateUI();
    }

    private void OpenMap()
    {
        remainingUses--;
        remainingTime = maxViewTime;

        isMapOpen = true;
        mapPanel.SetActive(true);
    }

    private void CloseMap()
    {
        isMapOpen = false;
        remainingTime = 0f;

        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }
    }

    // 使用回数を初期化
    public void ResetMapUsage()
    {
        remainingUses = maxUses;

        CloseMap();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (usesText != null)
        {
            usesText.text =
                "残り使用回数：" + remainingUses;
        }

        if (timerText != null)
        {
            timerText.text =
                "残り時間：" + Mathf.CeilToInt(remainingTime) + "秒";
        }
    }
}
