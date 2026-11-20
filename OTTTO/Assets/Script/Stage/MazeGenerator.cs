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
    [SerializeField] private float goalHeight = 1.5f;

    [Header("ゴールの壁設定")]
    [SerializeField]
    private GoalWallSide goalWallSide =
        GoalWallSide.Top;

    [SerializeField] private bool removeGoalWall = false;

    [Header("ゴールの位置調整")]
    [SerializeField] private float goalInset = 0.5f;
    [SerializeField] private bool rotateGoalTowardMaze = true;

    private enum GoalWallSide
    {
        Top,
        Right
    }

    private void Start()
    {
        // ステージ開始時に迷路生成
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

    // 迷路生成
    public void GenerateMaze()
    {
        if (width <= 0 || height <= 0 ||
            root == null || mazeCellPrefab == null ||
            cellScale <= 0f)
        {
            Debug.LogError(
                "MazeGeneratorの設定を確認してください");
            return;
        }

        // プレイヤーやゴールが削除対象に
        // 入っていないか確認する
        if ((player != null &&
             (player == root || player.IsChildOf(root))) ||
            (goal != null &&
             (goal == root || goal.IsChildOf(root))))
        {
            Debug.LogError(
                "PlayerとGoalはMazeRootの子にしないでください");
            return;
        }

        ClearMaze();

        // 迷路データを作成
        maze = new MazeCellModel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCellModel();
            }
        }

        // ランダムな迷路を生成
        GenerateMazePath(0, 0);

        // 必要な場合だけゴールの外壁を削除
        if (removeGoalWall)
        {
            RemoveGoalWall();
        }

        // 迷路のPrefabを配置
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
                        "MazeCellコンポーネントがありません");
                    continue;
                }

                cell.transform.localScale *= cellScale;
                cell.name = $"{x}-{y}";

                cell.Setup(maze[x, y]);
            }
        }

        // スタートとゴールの位置を自動設定
        SetStartAndGoal();

        Debug.Log("迷路を生成しました");
    }

    // 死亡時に呼び出す
    public void RestartMaze()
    {
        Debug.Log("死亡したため迷路を再生成します");

        // 新しい迷路を生成
        // プレイヤーもスタート位置に戻る
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

    // 探索方向をランダムに並べ替える
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

    // ゴール部分の外壁を削除
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
        // スタートの中心座標
        Vector3 startPosition = new Vector3(
            0f,
            playerHeight,
            0f
        );

        // StartPointも自動配置する
        if (startPoint != null)
        {
            startPoint.position = startPosition;
        }

        // プレイヤーをスタートに移動
        SetPlayerPosition(startPosition);

        // ゴールを右上の壁に配置
        SetGoalPosition();
    }

    // プレイヤーの位置を設定
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

        Physics.SyncTransforms();
    }

    // ゴールを右上の外壁に配置
    private void SetGoalPosition()
    {
        if (goal == null)
        {
            Debug.LogWarning(
                "Goalが設定されていません");
            return;
        }

        // 右上のマスの中心
        Vector3 goalCellCenter = new Vector3(
            (width - 1) * cellScale,
            0f,
            (height - 1) * cellScale
        );

        // 外壁の方向
        Vector3 wallDirection;

        if (goalWallSide == GoalWallSide.Top)
        {
            wallDirection = Vector3.forward;
        }
        else
        {
            wallDirection = Vector3.right;
        }

        // 外壁の中心座標
        Vector3 wallPosition =
            goalCellCenter +
            wallDirection * (cellScale / 2f);

        // 壁の内側に少し移動
        wallPosition -= wallDirection * goalInset;

        // 高さを設定
        wallPosition.y = goalHeight;

        goal.position = wallPosition;

        // ギミックを迷路の内側に向ける
        if (rotateGoalTowardMaze)
        {
            goal.rotation = Quaternion.LookRotation(
                -wallDirection,
                Vector3.up
            );
        }

        Debug.Log(
            "ゴールを配置しました：" + wallPosition);
    }
}