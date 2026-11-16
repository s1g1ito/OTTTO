
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[DefaultExecutionOrder(100)]
public class FogOfWar : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RawImage fogOverlay;

    [Header("探索設定")]
    [SerializeField] private int textureSize = 500;
    [SerializeField] private float startRevealRadius = 0.6f;
    [SerializeField] private float revealStep = 0.2f;

    [Header("通路の幅を検出")]
    [SerializeField] private LayerMask wallMask = ~0;
    [SerializeField] private float maxHalfWidth = 5f;
    [SerializeField] private float rayHeight = 1f;
    [SerializeField] private float wallPadding = 0.1f;

    [Header("ステージ全体の探索記録範囲")]
    [SerializeField]
    private Vector2 worldMin =
        new Vector2(-100f, -100f);

    [Header("セーブ設定")]
    [SerializeField] private string stageId = "Stage2";
    [SerializeField] private float autoSaveInterval = 3f;

    private bool saveDirty = false;
    private float lastSaveTime;

    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "fog_" + stageId + ".dat"
            );
        }
    }


    [SerializeField]
    private Vector2 worldMax =
        new Vector2(100f, 100f);

    private Texture2D fogTexture;
    private Color32[] fogPixels;
    private Vector3 lastPosition;
    private bool isDirty;

    private void Start()
    {
        if (player == null || minimapCamera == null ||
            fogOverlay == null || textureSize <= 0 ||
            worldMax.x <= worldMin.x ||
            worldMax.y <= worldMin.y)
        {
            Debug.LogError("FogOfWarの設定を確認してください");
            enabled = false;
            return;
        }

        fogTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        fogTexture.wrapMode = TextureWrapMode.Clamp;
        fogTexture.filterMode = FilterMode.Bilinear;

        fogPixels = new Color32[textureSize * textureSize];

        for (int i = 0; i < fogPixels.Length; i++)
        {
            fogPixels[i] = new Color32(0, 0, 0, 255);
        }

        fogOverlay.texture = fogTexture;
        fogOverlay.color = Color.white;


        lastPosition = player.position;

        // 前回の探索記録を読み込む
        LoadExploration();

        // 現在地周辺を探索済みにする
        RevealCircle(lastPosition, startRevealRadius);

        // マップを更新
        ApplyFog();
        UpdateMapView();

        lastSaveTime = Time.unscaledTime;

    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 currentPosition = player.position;

        Vector3 movement = currentPosition - lastPosition;
        movement.y = 0f;

        float distance = movement.magnitude;

        if (distance >= revealStep)
        {
            // テレポート時は途中の通路を開放しない
            if (distance > 5f)
            {
                RevealCircle(
                    currentPosition,
                    startRevealRadius
                );
            }
            else
            {
                Vector3 direction = movement.normalized;

                int steps = Mathf.CeilToInt(
                    distance / revealStep
                );

                for (int i = 1; i <= steps; i++)
                {
                    float t = (float)i / steps;

                    Vector3 samplePosition = Vector3.Lerp(
                        lastPosition,
                        currentPosition,
                        t
                    );

                    RevealCorridor(
                        samplePosition,
                        direction
                    );
                }
            }

            lastPosition = currentPosition;
            ApplyFog();
        }

        UpdateMapView();

        // 探索記録が変化していれば定期的に保存
        if (saveDirty &&
            Time.unscaledTime - lastSaveTime >=
            Mathf.Max(1f, autoSaveInterval))
        {
            SaveExploration();
        }

    }

    private void RevealCorridor(
        Vector3 position,
        Vector3 moveDirection)
    {
        // 進行方向に対して右方向
        Vector3 right = new Vector3(
            moveDirection.z,
            0f,
            -moveDirection.x
        );

        Vector3 left = -right;

        Vector3 rayOrigin = position +
            Vector3.up * rayHeight;

        float leftWidth = GetWallDistance(
            rayOrigin, left
        );

        float rightWidth = GetWallDistance(
            rayOrigin, right
        );

        // 通路の左端から右端まで探索済みにする
        for (float offset = -leftWidth;
             offset <= rightWidth;
             offset += revealStep)
        {
            Vector3 revealPosition =
                position + right * offset;

            ClearPixel(revealPosition);
        }

        ClearPixel(position + right * rightWidth);
    }

    private float GetWallDistance(
        Vector3 origin,
        Vector3 direction)
    {
        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            maxHalfWidth,
            wallMask,
            QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(
                0f,
                hit.distance - wallPadding
            );
        }

        return maxHalfWidth;
    }

    private void RevealCircle(
        Vector3 position,
        float radius)
    {
        for (float x = -radius; x <= radius;
             x += revealStep)
        {
            for (float z = -radius; z <= radius;
                 z += revealStep)
            {
                if (x * x + z * z > radius * radius)
                    continue;

                ClearPixel(
                    position + new Vector3(x, 0f, z)
                );
            }
        }

        ClearPixel(position);
    }

    private void ClearPixel(Vector3 worldPosition)
    {
        float nx = (
            worldPosition.x - worldMin.x
        ) / (worldMax.x - worldMin.x);

        float ny = (
            worldPosition.z - worldMin.y
        ) / (worldMax.y - worldMin.y);

        if (nx < 0f || nx > 1f ||
            ny < 0f || ny > 1f)
        {
            return;
        }

        int px = Mathf.RoundToInt(
            nx * (textureSize - 1)
        );

        int py = Mathf.RoundToInt(
            ny * (textureSize - 1)
        );

        int index = py * textureSize + px;

        if (fogPixels[index].a == 0)
            return;

        fogPixels[index] = new Color32(0, 0, 0, 0);

        // 地図の画像を更新する
        isDirty = true;

        // 探索記録を保存する
        saveDirty = true;
    }

    private void ApplyFog()
    {
        if (!isDirty) return;

        fogTexture.SetPixels32(fogPixels);
        fogTexture.Apply(false);

        isDirty = false;
    }

    private void UpdateMapView()
    {
        float viewHeight =
            minimapCamera.orthographicSize * 2f;

        float viewWidth =
            viewHeight * minimapCamera.aspect;

        float worldWidth = worldMax.x - worldMin.x;
        float worldHeight = worldMax.y - worldMin.y;

        Vector3 cameraPos =
            minimapCamera.transform.position;

        float u =
            (cameraPos.x - worldMin.x) / worldWidth;

        float v =
            (cameraPos.z - worldMin.y) / worldHeight;

        float uvWidth = viewWidth / worldWidth;
        float uvHeight = viewHeight / worldHeight;

        fogOverlay.uvRect = new Rect(
            u - uvWidth * 0.5f,
            v - uvHeight * 0.5f,
            uvWidth,
            uvHeight
        );
    }


    [ContextMenu("探索記録をリセット")]
    public void ResetExploration()
    {
        if (fogTexture == null ||
            fogPixels == null ||
            player == null)
        {
            return;
        }

        // 探索記録をすべて黒に戻す
        for (int i = 0; i < fogPixels.Length; i++)
        {
            fogPixels[i] = new Color32(0, 0, 0, 255);
        }

        // 現在地から探索を再開
        lastPosition = player.position;

        // 地図と保存データを更新対象にする
        isDirty = true;
        saveDirty = true;

        // 現在地周辺を表示
        RevealCircle(lastPosition, startRevealRadius);

        // 地図を更新
        ApplyFog();
        UpdateMapView();

        // リセット後の状態を保存
        SaveExploration();

        Debug.Log("探索記録をリセットしました！");
    }


    [ContextMenu("探索記録を保存")]
    public void SaveExploration()
    {
        if (fogPixels == null || !saveDirty)
            return;

        try
        {
            byte[] alphaData = new byte[fogPixels.Length];

            for (int i = 0; i < fogPixels.Length; i++)
            {
                alphaData[i] = fogPixels[i].a;
            }

            Directory.CreateDirectory(
                Application.persistentDataPath
            );

            using (BinaryWriter writer =
                new BinaryWriter(File.Create(SavePath)))
            {
                // 保存形式のバージョン
                writer.Write(1);

                // マップ設定
                writer.Write(textureSize);
                writer.Write(worldMin.x);
                writer.Write(worldMin.y);
                writer.Write(worldMax.x);
                writer.Write(worldMax.y);

                // 探索済み・未探索の情報
                writer.Write(alphaData);
            }

            saveDirty = false;
            lastSaveTime = Time.unscaledTime;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "探索記録の保存に失敗: " + e.Message
            );
        }
    }

    public void LoadExploration()
    {
        if (fogPixels == null || !File.Exists(SavePath))
            return;

        try
        {
            using (BinaryReader reader =
                new BinaryReader(File.OpenRead(SavePath)))
            {
                int version = reader.ReadInt32();
                int savedSize = reader.ReadInt32();

                float minX = reader.ReadSingle();
                float minY = reader.ReadSingle();
                float maxX = reader.ReadSingle();
                float maxY = reader.ReadSingle();

                // 現在のマップ設定と一致するか確認
                if (version != 1 ||
                    savedSize != textureSize ||
                    minX != worldMin.x ||
                    minY != worldMin.y ||
                    maxX != worldMax.x ||
                    maxY != worldMax.y)
                {
                    Debug.LogWarning(
                        "保存データとマップ設定が一致しません"
                    );
                    return;
                }

                byte[] alphaData =
                    reader.ReadBytes(fogPixels.Length);

                if (alphaData.Length != fogPixels.Length)
                {
                    Debug.LogWarning(
                        "探索記録のデータが不完全です"
                    );
                    return;
                }

                for (int i = 0; i < fogPixels.Length; i++)
                {
                    fogPixels[i] = new Color32(
                        0, 0, 0, alphaData[i]
                    );
                }
            }

            isDirty = true;

            Debug.Log(
                "探索記録を読み込みました: " + stageId
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "探索記録の読み込みに失敗: " + e.Message
            );
        }
    }

    // シーン切り替え・オブジェクト無効化時に保存
    private void OnDisable()
    {
        SaveExploration();
    }

    // アプリ終了時にも保存
    private void OnApplicationQuit()
    {
        SaveExploration();
    }

    private void OnDestroy()
    {
        if (fogTexture != null)
        {
            Destroy(fogTexture);
        }
    }
}
