using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScout : MonoBehaviour
{
    [Header("視認設定")]
    public float viewDistance = 5f;
    public float viewAngle = 45f;
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
    private bool foundPlayer = false;

    private Mesh visionMesh;
    private MeshRenderer visionRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);

        CreateVisionMesh();
    }

    void Update()
    {
        if (player != null)
        {
            bool canSeePlayer = CanSeePlayer();

            if (canSeePlayer)
            {
                // Playerを発見 → 赤
                SetVisionColor(detectedViewColor);

                if (!foundPlayer)
                {
                    foundPlayer = true;

                    if (hunter != null)
                        hunter.StartChase();

                    Debug.Log("敵AがPlayerを発見！");
                }
            }
            else
            {
                // Playerが視界外 → 緑
                SetVisionColor(normalViewColor);
            }

            Patrol();
        }

        UpdateVisionMesh();
    }

    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0) return;

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) % patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position);
        }
    }

    bool CanSeePlayer()
    {
        // 敵の目の高さ
        Vector3 origin =
            transform.position + Vector3.up * rayHeight;

        // Playerの少し上を狙う
        Vector3 target =
            player.position + Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;

        // 水平方向だけで距離・角度を計算
        Vector3 flatDirection =
            new Vector3(direction.x, 0, direction.z);

        float distance = flatDirection.magnitude;

        // 距離チェック
        if (distance > viewDistance)
        {
            return false;
        }

        // 角度チェック
        float angle =
            Vector3.Angle(transform.forward, flatDirection);

        if (angle > viewAngle)
        {
            return false;
        }

        // Playerへ向かってRayを飛ばす
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

            // Playerに到達する前に壁などに当たった
            return false;
        }

        return false;
    }

    void SetVisionColor(Color color)
    {
        if (visionRenderer == null)
            return;

        Material mat = visionRenderer.material;

        // URP
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        // Built-in
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
    }

    void CreateVisionMesh()
    {
        GameObject vision = new GameObject("VisionRange");

        vision.transform.SetParent(transform);
        vision.transform.localPosition = new Vector3(0, 3.0f, 0);
        vision.transform.localRotation = Quaternion.identity;

        MeshFilter filter = vision.AddComponent<MeshFilter>();
        visionRenderer = vision.AddComponent<MeshRenderer>();

        visionMesh = new Mesh();
        visionMesh.name = "VisionMesh";

        filter.mesh = visionMesh;

        // URP用のUnlitシェーダー
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");

        if (shader == null)
        {
            // URPではない場合
            shader = Shader.Find("Unlit/Color");
        }

        Material mat = new Material(shader);

        // 緑色
        Color green = normalViewColor;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", green);

        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", green);

        // 両面表示
        if (mat.HasProperty("_Cull"))
            mat.SetFloat("_Cull", 0);

        mat.renderQueue = 3000;

        visionRenderer.material = mat;

        visionRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        visionRenderer.receiveShadows = false;
    }

    void UpdateVisionMesh()
    {
        if (visionMesh == null) return;

        int count = segments + 2;

        Vector3[] vertices = new Vector3[count];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle =
                -viewAngle + (viewAngle * 2f * i / segments);

            float rad = angle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(rad) * viewDistance,
                0,
                Mathf.Cos(rad) * viewDistance
            );

            if (i < segments)
            {
                int t = i * 3;

                triangles[t] = 0;
                triangles[t + 1] = i + 1;
                triangles[t + 2] = i + 2;
            }
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
        visionMesh.RecalculateBounds();
    }
}
