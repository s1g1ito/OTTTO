using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("迷路生成設定")]
    [Min(1)] public int width = 10;
    [Min(1)] public int height = 10;

    private System.Random random = new System.Random();
    private MazeCellModel[,] maze;

    public GameObject mazeCellPrefab;

    [SerializeField] private Transform root;
    [SerializeField] private float cellScale = 5f;

    [Header("スタート設定")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float playerHeight = 1f;

    [Header("ゴール設定")]
    [SerializeField] private Transform goal;
    [SerializeField] private float goalHeight = 0f;

    [Header("ゴールの壁設定")]
    [SerializeField]
    private GoalWallSide goalWallSide =
        GoalWallSide.Top;

    [SerializeField] private bool removeGoalWall = true;

    [Header("ゴールの位置調整")]
    [SerializeField] private float goalInset = 0f;
    [SerializeField] private bool rotateGoalTowardMaze = true;

    [Header("感圧板ギミック設定")]
    [SerializeField] private PlateCheak pressurePlate;
    [SerializeField] private Rigidbody keyBox;
    [SerializeField] private Transform keyBoxStartPoint;

    private Vector3 initialKeyBoxPosition;
    private Quaternion initialKeyBoxRotation;

    private enum GoalWallSide
    {
        Top,
        Right
    }

    private void Awake()
    {
        // 箱の初期位置を保存
        if (keyBox != null)
        {
            initialKeyBoxPosition = keyBox.position;
            initialKeyBoxRotation = keyBox.rotation;
        }
    }

    private void Start()
    {
        GenerateMaze();
    }

    // 既存の迷路を削除
    public void ClearMaze()
    {
        if (root == null)
        {
            return;
        }

        List<GameObject> tempList = new List<GameObject>();

        foreach (Transform child in root)
        {
            tempList.Add(child.gameObject);
        }

        foreach (GameObject obj in tempList)
        {
            if (Application.isPlaying)
            {
                obj.SetActive(false);
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }
    }

    // 迷路をランダム生成
    public void GenerateMaze()
    {
        if (width <= 0 || height <= 0 ||
            root == null ||
            mazeCellPrefab == null ||
            cellScale <= 0f)
        {
            Debug.LogError(
                "MazeGeneratorの設定を確認してください");
            return;
        }

        // 削除対象に重要なオブジェクトが
        // 入っていないか確認する
        if (IsInsideMazeRoot(player) ||
            IsInsideMazeRoot(startPoint) ||
            IsInsideMazeRoot(goal) ||
            (pressurePlate != null &&
             IsInsideMazeRoot(pressurePlate.transform)) ||
            (keyBox != null &&
             IsInsideMazeRoot(keyBox.transform)))
        {
            Debug.LogError(
                "Player、Goal、箱、感圧板などを" +
                "MazeRootの子にしないでください");
            return;
        }

        ClearMaze();

        // 迷路データ作成
        maze = new MazeCellModel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCellModel();
            }
        }

        // 深さ優先探索で迷路生成
        GenerateMazePath(0, 0);

        // ゴールの外壁を削除
        if (removeGoalWall)
        {
            RemoveGoalWall();
        }

        // 迷路のPrefabを生成
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = x * cellScale;
                float posZ = y * cellScale;

                GameObject obj = Instantiate(
                    mazeCellPrefab,
                    new Vector3(posX, 0f, posZ),
                    Quaternion.identity,
                    root
                );

                MazeCell cell = obj.GetComponent<MazeCell>();

                if (cell == null)
                {
                    Debug.LogError(
                        "MazeCellが設定されていません");
                    continue;
                }

                cell.transform.localScale *= cellScale;
                cell.name = $"{x}-{y}";

                cell.Setup(maze[x, y]);
            }
        }

        // スタートとゴールを自動配置
        SetStartAndGoal();

        // 箱と感圧板を初期化
        ResetGimmick();

        Physics.SyncTransforms();

        Debug.Log("迷路とギミックを生成しました");
    }

    // MazeRootの子かどうかを確認
    private bool IsInsideMazeRoot(Transform target)
    {
        if (target == null || root == null)
        {
            return false;
        }

        return target == root || target.IsChildOf(root);
    }

    // 死亡時に呼び出す
    public void RestartMaze()
    {
        Debug.Log("迷路を再生成します");

        GenerateMaze();
    }

    // 深さ優先探索
    private void GenerateMazePath(int x, int y)
    {
        MazeCellModel currentCell = maze[x, y];
        currentCell.visited = true;

        foreach (var direction in ShuffleDirections())
        {
            int newX = x + direction.Item1;
            int newY = y + direction.Item2;

            if (newX >= 0 && newY >= 0 &&
                newX < width && newY < height)
            {
                MazeCellModel neighbourCell = maze[newX, newY];

                if (!neighbourCell.visited)
                {
                    neighbourCell.visited = true;

                    currentCell.RemoveWall(direction.Item3);
                    neighbourCell.RemoveWall(direction.Item4);

                    GenerateMazePath(newX, newY);
                }
            }
        }
    }

    // 探索方向をランダム化
    private List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)>
        ShuffleDirections()
    {
        List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)>
            directions =
            new List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)>
        {
            (0, 1,
                MazeCellModel.Wall.Top,
                MazeCellModel.Wall.Bottom),

            (0, -1,
                MazeCellModel.Wall.Bottom,
                MazeCellModel.Wall.Top),

            (-1, 0,
                MazeCellModel.Wall.Left,
                MazeCellModel.Wall.Right),

            (1, 0,
                MazeCellModel.Wall.Right,
                MazeCellModel.Wall.Left)
        };

        for (int i = 0; i < directions.Count; i++)
        {
            var temp = directions[i];

            int randomIndex = random.Next(
                i, directions.Count);

            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        return directions;
    }

    // ゴールの外壁を削除
    private void RemoveGoalWall()
    {
        MazeCellModel goalCell =
            maze[width - 1, height - 1];

        if (goalWallSide == GoalWallSide.Top)
        {
            goalCell.RemoveWall(
                MazeCellModel.Wall.Top);
        }
        else
        {
            goalCell.RemoveWall(
                MazeCellModel.Wall.Right);
        }
    }

    // スタートとゴールを自動配置
    private void SetStartAndGoal()
    {
        Vector3 startPosition = new Vector3(
            0f,
            playerHeight,
            0f
        );

        if (startPoint != null)
        {
            startPoint.position = startPosition;
        }

        SetPlayerPosition(startPosition);

        SetGoalPosition();
    }

    // プレイヤー配置
    private void SetPlayerPosition(Vector3 position)
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            Debug.LogWarning(
                "Playerが設定されていません");
            return;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.position = position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            player.position = position;
        }
    }

    // 右上の外壁にゴールを配置
    private void SetGoalPosition()
    {
        if (goal == null)
        {
            Debug.LogWarning(
                "Goalが設定されていません");
            return;
        }

        Vector3 goalCellCenter = new Vector3(
            (width - 1) * cellScale,
            0f,
            (height - 1) * cellScale
        );

        Vector3 wallDirection =
            goalWallSide == GoalWallSide.Top
            ? Vector3.forward
            : Vector3.right;

        Vector3 wallPosition =
            goalCellCenter +
            wallDirection * (cellScale / 2f);

        wallPosition -= wallDirection * goalInset;
        wallPosition.y = goalHeight;

        goal.position = wallPosition;

        if (rotateGoalTowardMaze)
        {
            goal.rotation = Quaternion.LookRotation(
                -wallDirection,
                Vector3.up
            );
        }

        // 扉の配置位置を初期位置として再設定
        DoorMove[] doors =
            goal.GetComponentsInChildren<DoorMove>(true);

        foreach (DoorMove door in doors)
        {
            // Goal自身が扉なら位置を維持
            // 子オブジェクトなら初期ローカル位置に戻す
            bool isChild = door.transform != goal;

            door.InitializeDoor(isChild);
        }

        Debug.Log(
            "ゴールを配置しました：" + wallPosition);
    }

    // 箱と感圧板の初期化
    private void ResetGimmick()
    {
        // 箱を元の位置に戻す
        if (keyBox != null)
        {
            Vector3 boxPosition =
                keyBoxStartPoint != null
                ? keyBoxStartPoint.position
                : initialKeyBoxPosition;

            Quaternion boxRotation =
                keyBoxStartPoint != null
                ? keyBoxStartPoint.rotation
                : initialKeyBoxRotation;

            keyBox.linearVelocity = Vector3.zero;
            keyBox.angularVelocity = Vector3.zero;

            keyBox.position = boxPosition;
            keyBox.rotation = boxRotation;
        }

        // 感圧板の判定をリセット
        if (pressurePlate != null)
        {
            pressurePlate.ResetPlate();
        }
    }
}