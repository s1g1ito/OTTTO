using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScout : MonoBehaviour
{
    [Header("視認設定")]
    public float viewDistance = 10f;

    // 扇形全体の角度
    // 60なら左30°～右30°
    public float viewAngle = 60f;

    public float rayHeight = 1f;

    [Header("徘徊設定")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("通知する敵B")]
    public EnemyHunter hunter;

    [Header("視界を遮るレイヤー")]
    [Tooltip("Wallなど、視界を遮るLayerだけを設定してください。Groundは入れません。")]
    public LayerMask visionObstacleMask;

    [Header("視界表示設定")]
    public Color normalViewColor =
        new Color(0f, 1f, 0f, 0.5f);

    public Color detectedViewColor =
        new Color(1f, 0f, 0f, 0.7f);

    [Header("VisionRange表示")]
    public float groundHeight = 3.0f;

    [Range(10, 100)]
    public int segments = 40;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;

    // 現在Playerを視認しているか
    private bool foundPlayer = false;

    private Mesh visionMesh;
    private MeshRenderer visionRenderer;

    void Start()
    {
        // ========================================
        // NavMeshAgent
        // ========================================

        agent = GetComponent<NavMeshAgent>();

        // ========================================
        // Player取得
        // ========================================

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;

            Debug.Log(
                "EnemyScout：Playerを取得しました"
            );
        }
        else
        {
            Debug.LogError(
                "EnemyScout：Playerタグのオブジェクトが見つかりません！"
            );
        }

        // ========================================
        // EnemyHunter取得
        // ========================================

        if (hunter == null)
        {
            hunter =
                Object.FindAnyObjectByType<EnemyHunter>();

            if (hunter != null)
            {
                Debug.Log(
                    "EnemyScout：EnemyHunterを自動取得しました"
                );
            }
            else
            {
                Debug.LogError(
                    "EnemyScout：EnemyHunterが見つかりません！"
                );
            }
        }

        // ========================================
        // NavMesh確認
        // ========================================

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "EnemyScout：NavMesh上にいません！"
            );
        }

        // ========================================
        // 最初の巡回地点
        // ========================================

        if (patrolPoints != null &&
            patrolPoints.Length > 0 &&
            agent.isOnNavMesh)
        {
            agent.SetDestination(
                patrolPoints[0].position
            );
        }

        // ========================================
        // VisionRange作成
        // ========================================

        CreateVisionMesh();

        // 最初は緑
        SetVisionColor(normalViewColor);
    }

    void Update()
    {
        // ========================================
        // Playerがいない
        // ========================================

        if (player == null)
        {
            Patrol();
            UpdateVisionMesh();
            return;
        }

        // ========================================
        // Player視認
        // ========================================

        bool canSeePlayer =
            CanSeePlayer();

        // ========================================
        // 発見中
        // ========================================

        if (canSeePlayer)
        {
            // VisionRangeを赤にする
            SetVisionColor(
                detectedViewColor
            );

            Debug.Log("EnemyScout：canSeePlayer = TRUE / foundPlayer = " + foundPlayer);

            // 初めて発見した瞬間
            if (!foundPlayer)
            {
                foundPlayer = true;

                Debug.Log(
                    "================================"
                );

                Debug.Log(
                    "敵A：Playerを発見！"
                );

                Debug.Log(
                    "VisionRange → 赤"
                );

                // EnemyHunterへ通知
                if (hunter != null)
                {
                    Debug.Log(
                        "敵A → 敵Bへ追跡通知！"
                    );

                    hunter.StartChase();
                }
                else
                {
                    Debug.LogError(
                        "EnemyScout：EnemyHunterが設定されていません！"
                    );
                }

                Debug.Log(
                    "================================"
                );
            }
        }
        else
        {
            // 見失ったら緑
            SetVisionColor(
                normalViewColor
            );

            // 視界から出た
            if (foundPlayer)
            {
                foundPlayer = false;

                Debug.Log(
                    "EnemyScout：PlayerがVisionRangeから出ました"
                );
            }
        }

        // ========================================
        // 巡回
        // ========================================

        Patrol();

        // ========================================
        // VisionRange更新
        // ========================================

        UpdateVisionMesh();
    }

    // ========================================
    // 巡回
    // ========================================

    void Patrol()
    {
        if (agent == null ||
            !agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        if (patrolPoints[currentPoint] == null)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) %
                patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position
            );
        }
    }

    // ========================================
    // Player視認判定
    // ========================================

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;

        Vector3 target =
            player.position +
            Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;

        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );

        float distance =
            flatDirection.magnitude;

        // 距離チェック
        if (distance > viewDistance)
        {
            return false;
        }

        // 角度チェック
        float angle =
            Vector3.Angle(
                transform.forward,
                flatDirection
            );

        if (angle > viewAngle / 2f)
        {
            return false;
        }

        Vector3 rayDirection =
            direction.normalized;

        float rayDistance =
            direction.magnitude;

        Debug.DrawRay(
            origin,
            rayDirection * rayDistance,
            Color.yellow
        );

        // Rayが何に当たったか全部取得
        RaycastHit[] hits =
            Physics.RaycastAll(
                origin,
                rayDirection,
                rayDistance
            );

        // 距離順に並べる
        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject =
                hit.collider.gameObject;

            string layerName =
                LayerMask.LayerToName(
                    hitObject.layer
                );

            Debug.Log(
                "EnemyScout：Raycast → " +
                hitObject.name +
                " / Layer = " +
                layerName
            );

            // ========================================
            // Playerに当たった
            // ========================================

            if (hitObject.CompareTag("Player"))
            {
                Debug.Log(
                    "EnemyScout：Playerを視認しました！"
                );

                // ★ここが重要
                return true;
            }

            // ========================================
            // Groundは無視
            // ========================================

            if (layerName == "Ground")
            {
                continue;
            }

            // ========================================
            // Wallなら視界を遮る
            // ========================================

            if (layerName == "Wall")
            {
                Debug.Log(
                    "EnemyScout：Wallに視界を遮られました"
                );

                return false;
            }

            // その他のオブジェクトは無視
        }

        // Playerに当たらなかった
        return false;
    }

    // ========================================
    // VisionRange色変更
    // ========================================

    void SetVisionColor(Color color)
    {
        if (visionRenderer == null)
        {
            return;
        }

        Material mat =
            visionRenderer.material;

        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor(
                "_BaseColor",
                color
            );
        }

        if (mat.HasProperty("_Color"))
        {
            mat.SetColor(
                "_Color",
                color
            );
        }
    }

    // ========================================
    // VisionRange作成
    // ========================================

    void CreateVisionMesh()
    {
        GameObject vision =
            new GameObject("VisionRange");

        vision.transform.SetParent(
            transform
        );

        vision.transform.localPosition =
            new Vector3(
                0f,
                groundHeight,
                0f
            );

        vision.transform.localRotation =
            Quaternion.identity;

        MeshFilter filter =
            vision.AddComponent<MeshFilter>();

        visionRenderer =
            vision.AddComponent<MeshRenderer>();

        visionMesh =
            new Mesh();

        visionMesh.name =
            "VisionMesh";

        filter.mesh =
            visionMesh;

        // ========================================
        // Shader
        // ========================================

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
        {
            shader =
                Shader.Find("Unlit/Color");
        }

        Material mat =
            new Material(shader);

        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor(
                "_BaseColor",
                normalViewColor
            );
        }

        if (mat.HasProperty("_Color"))
        {
            mat.SetColor(
                "_Color",
                normalViewColor
            );
        }

        // 両面表示
        if (mat.HasProperty("_Cull"))
        {
            mat.SetFloat(
                "_Cull",
                0
            );
        }

        mat.renderQueue = 3000;

        visionRenderer.material =
            mat;

        visionRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        visionRenderer.receiveShadows =
            false;
    }

    // ========================================
    // VisionRange Mesh更新
    // ========================================

    void UpdateVisionMesh()
    {
        if (visionMesh == null)
        {
            return;
        }

        int count =
            segments + 2;

        Vector3[] vertices =
            new Vector3[count];

        int[] triangles =
            new int[segments * 3];

        // 中心
        vertices[0] =
            Vector3.zero;

        // ========================================
        // 扇形
        // ========================================

        for (int i = 0;
             i <= segments;
             i++)
        {
            float angle =
                -viewAngle / 2f +
                (
                    viewAngle *
                    i /
                    segments
                );

            float rad =
                angle *
                Mathf.Deg2Rad;

            vertices[i + 1] =
                new Vector3(
                    Mathf.Sin(rad) *
                    viewDistance,

                    0f,

                    Mathf.Cos(rad) *
                    viewDistance
                );

            if (i < segments)
            {
                int t =
                    i * 3;

                triangles[t] =
                    0;

                triangles[t + 1] =
                    i + 1;

                triangles[t + 2] =
                    i + 2;
            }
        }

        visionMesh.Clear();

        visionMesh.vertices =
            vertices;

        visionMesh.triangles =
            triangles;

        visionMesh.RecalculateNormals();

        visionMesh.RecalculateBounds();
    }
}