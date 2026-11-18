using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScout : MonoBehaviour
{
    [Header("視認設定")]
    public float viewDistance = 10f;
    public float viewAngle = 60f;
    public float rayHeight = 1f;

    [Header("徘徊設定")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("通知する敵B")]
    public EnemyHunter hunter;

    [Header("視界表示設定")]
    public Color normalViewColor = new Color(0f, 1f, 0f, 0.5f);
    public Color detectedViewColor = new Color(1f, 0f, 0f, 0.7f);

    public float groundHeight = 3.0f;

    [Range(10, 100)]
    public int segments = 40;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;

    // 一度発見したら、同じPlayerに何度も通知しない
    private bool foundPlayer = false;

    private Mesh visionMesh;
    private MeshRenderer visionRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Player取得
        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;
        }
        else
        {
            Debug.LogError("EnemyScout：Playerタグのオブジェクトが見つかりません！");
        }

        // 最初の徘徊地点へ
        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            agent.SetDestination(
                patrolPoints[0].position
            );
        }

        // 視界表示作成
        CreateVisionMesh();
    }

    void Update()
    {
        if (player != null)
        {

            // 敵A自身の視界判定
            bool canSeePlayer = CanSeePlayer();

            if (canSeePlayer)
            {
                // Playerを発見
                if (!foundPlayer)
                {
                    foundPlayer = true;

                    Debug.Log(
                        "★★ 敵Aの扇形にPlayerが入った！ ★★"
                    );

                    // 敵Bへ通知
                    if (hunter != null)
                    {
                        Debug.Log(
                            "★★ 敵A → 敵Bへ追跡通知！ ★★"
                        );

                        hunter.StartChase();
                    }
                    else
                    {
                        Debug.LogWarning(
                            "EnemyScout：EnemyHunterが設定されていません！"
                        );
                    }
                }

                // 色変更は後で調整
                SetVisionColor(detectedViewColor);
            }
            else
            {
                SetVisionColor(normalViewColor);
            }

            // 徘徊
            Patrol();
        }

        // 扇形表示更新
        UpdateVisionMesh();
    }

    // 徘徊
    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) % patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position
            );
        }
    }

    // Player視認判定
    bool CanSeePlayer()
    {
        // 敵Aの目の高さ
        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;

        // Playerの胸あたりを狙う
        Vector3 target =
            player.position +
            Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;

        // 水平方向だけ取得
        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );

        float distance =
            flatDirection.magnitude;

        //距離判定

        if (distance > viewDistance)
        {
            return false;
        }

        //扇形の角度判定

        float angle =
            Vector3.Angle(
                transform.forward,
                flatDirection
            );

        if (angle > viewAngle)
        {
            return false;
        }

        //遮蔽物判定

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            direction.magnitude))
        {
            // Player本体
            if (hit.transform == player)
            {
                return true;
            }

            // Playerの子オブジェクト
            if (hit.transform.IsChildOf(player))
            {
                return true;
            }

            // Playerより先に壁などに当たった
            return false;
        }

        return false;
    }

    // 視界の色変更

    void SetVisionColor(Color color)
    {
        if (visionRenderer == null)
        {
            return;
        }

        Material mat =
            visionRenderer.material;

        // URP
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor(
                "_BaseColor",
                color
            );
        }

        // Built-in
        if (mat.HasProperty("_Color"))
        {
            mat.SetColor(
                "_Color",
                color
            );
        }
    }

    // 視界Mesh作成

    void CreateVisionMesh()
    {
        GameObject vision =
            new GameObject("VisionRange");

        vision.transform.SetParent(transform);

        vision.transform.localPosition =
            new Vector3(
                0,
                groundHeight,
                0
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

        // URP
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        // Built-in
        if (shader == null)
        {
            shader =
                Shader.Find("Unlit/Color");
        }

        Material mat =
            new Material(shader);

        // 通常時は緑
        Color green =
            normalViewColor;

        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor(
                "_BaseColor",
                green
            );
        }

        if (mat.HasProperty("_Color"))
        {
            mat.SetColor(
                "_Color",
                green
            );
        }

        // 両面表示
        if (mat.HasProperty("_Cull"))
        {
            mat.SetFloat("_Cull", 0);
        }

        mat.renderQueue = 3000;

        visionRenderer.material =
            mat;

        visionRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        visionRenderer.receiveShadows =
            false;
    }

    // 視界Mesh更新

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

        for (int i = 0; i <= segments; i++)
        {
            float angle =
                -viewAngle +
                (
                    viewAngle * 2f * i /
                    segments
                );

            float rad =
                angle * Mathf.Deg2Rad;

            vertices[i + 1] =
                new Vector3(
                    Mathf.Sin(rad) *
                    viewDistance,

                    0,

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