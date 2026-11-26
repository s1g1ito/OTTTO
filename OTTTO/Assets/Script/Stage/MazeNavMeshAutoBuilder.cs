
using UnityEngine;
using Unity.AI.Navigation;

/// <summary>
/// 迷路が生成・再生成されたときだけ、そのステージのNavMeshを作り直します。
/// MazeGenerator本体の変更は不要です。
/// </summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public class MazeNavMeshAutoBuilder : MonoBehaviour
{
    [Header("共通の迷路生成スクリプト")]
    [SerializeField] private MazeGenerator mazeGenerator;

    [Header("迷路の生成先（MazeGeneratorのRootと同じ）")]
    [SerializeField] private Transform mazeRoot;

    [Header("このステージ専用のNavMeshSurface")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("デバッグ表示")]
    [SerializeField] private bool showBuildLog = true;

    // 再生成後は新しいMazeCellインスタンスになるので、
    // そのInstance IDの変化で迷路の作り直しを検知します。
    private int lastBuiltFirstCellId;
    private bool hasBuiltNavMesh;
    private bool hasWarnedMissingReferences;

    private void Awake()
    {
        AssignReferencesIfPossible();
    }

    private void OnEnable()
    {
        // 一度無効化して再び有効化した場合も再構築する
        hasBuiltNavMesh = false;
        lastBuiltFirstCellId = 0;
    }

    private void Start()
    {
        // 通常はMazeGenerator.Start()後に呼ばれる
        TryBuildForCurrentMaze();
    }

    private void LateUpdate()
    {
        // 同じ迷路ならBakeを繰り返さない
        TryBuildForCurrentMaze();
    }

    private void OnDisable()
    {
        // このSurfaceのNavMeshだけを登録解除する
        if (navMeshSurface != null)
        {
            navMeshSurface.RemoveData();
        }

        hasBuiltNavMesh = false;
        lastBuiltFirstCellId = 0;
    }

    private void AssignReferencesIfPossible()
    {
        if (mazeGenerator == null)
        {
            mazeGenerator = GetComponent<MazeGenerator>();
        }

        if (navMeshSurface == null && mazeRoot != null)
        {
            navMeshSurface =
                mazeRoot.GetComponent<NavMeshSurface>();
        }
    }

    private bool CheckSettings()
    {
        AssignReferencesIfPossible();

        string problem = null;

        if (mazeGenerator == null)
        {
            problem = "Maze Generator が未設定です";
        }
        else if (mazeRoot == null)
        {
            problem = "Maze Root が未設定です";
        }
        else if (navMeshSurface == null)
        {
            problem = "Nav Mesh Surface が未設定です";
        }
        else if (!navMeshSurface.isActiveAndEnabled)
        {
            problem = "NavMeshSurface が無効になっています";
        }
        else if (navMeshSurface.gameObject.scene.handle !=
                 gameObject.scene.handle)
        {
            problem =
                "NavMeshSurface と MazeManager が別のSceneにあります";
        }

        if (problem != null)
        {
            if (!hasWarnedMissingReferences)
            {
                Debug.LogError(
                    "MazeNavMeshAutoBuilder: " + problem,
                    this
                );

                hasWarnedMissingReferences = true;
            }

            return false;
        }

        hasWarnedMissingReferences = false;
        return true;
    }

    private void TryBuildForCurrentMaze()
    {
        if (!CheckSettings())
        {
            return;
        }

        int firstCellId = 0;

        // 無効化された古いマスは無視する
        foreach (Transform child in mazeRoot)
        {
            if (!child.gameObject.activeInHierarchy ||
                !child.TryGetComponent<MazeCell>(out _))
            {
                continue;
            }

            firstCellId = child.GetInstanceID();
            break;
        }

        // 迷路がまだない場合
        if (firstCellId == 0)
        {
            return;
        }

        // 前回と同じ迷路なら再生成不要
        if (hasBuiltNavMesh &&
            firstCellId == lastBuiltFirstCellId)
        {
            return;
        }

        // 新しい迷路のマスが全部揃ったか確認
        int activeCellCount = 0;

        foreach (Transform child in mazeRoot)
        {
            if (child.gameObject.activeInHierarchy &&
                child.TryGetComponent<MazeCell>(out _))
            {
                activeCellCount++;
            }
        }

        int expectedCellCount =
            mazeGenerator.width * mazeGenerator.height;

        if (activeCellCount != expectedCellCount)
        {
            return;
        }

        BuildCurrentNavMesh(firstCellId);
    }

    private void BuildCurrentNavMesh(int firstCellId)
    {
        // 新しい床・壁のCollider位置を反映
        Physics.SyncTransforms();

        // 以前のNavMeshを登録解除
        navMeshSurface.RemoveData();

        // 現在の迷路に合わせてNavMeshを生成
        navMeshSurface.BuildNavMesh();

        if (navMeshSurface.navMeshData == null)
        {
            Debug.LogError(
                "NavMeshDataを生成できませんでした。" +
                "Surfaceの収集設定と床を確認してください。",
                this
            );
            return;
        }

        hasBuiltNavMesh = true;
        lastBuiltFirstCellId = firstCellId;

        if (showBuildLog)
        {
            Debug.Log(
                "[" + gameObject.scene.name +
                "] 迷路に合わせてNavMeshを生成しました。" +
                " 実際の通行可能領域はSceneビューで確認してください。",
                this
            );
        }
    }

    // 外部から手動で再生成する場合にも使える
    public void RebuildNow()
    {
        hasBuiltNavMesh = false;
        TryBuildForCurrentMaze();
    }
}
