using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("迷路生成設定")]
    public int width, height;
    private System.Random random = new System.Random();
    private MazeCellModel[,] maze;

    public GameObject mazeCellPrefab;
    [SerializeField] private Transform root;
    [SerializeField] private float cellScale = 5f;

    [Header("スタート・ゴール設定")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform goal;
    [SerializeField] private float playerHeight = 1f;
    [SerializeField] private float goalHeight = 0.5f;

    private void Start()
    {
        GenerateMaze();
    }

    // 既存の迷路を削除
    public void ClearMaze()
    {
        List<GameObject> tempList = new List<GameObject>();

        foreach (Transform child in root)
        {
            tempList.Add(child.gameObject);
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            if (Application.isPlaying)
            {
                tempList[i].SetActive(false);
                Destroy(tempList[i]);
            }
            else
            {
                DestroyImmediate(tempList[i]);
            }
        }
    }

    // 迷路の生成
    public void GenerateMaze()
    {
        ClearMaze();

        maze = new MazeCellModel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCellModel();
            }
        }

        GenerateMaze(0, 0);

        // 迷路のPrefabを配置
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = x * cellScale;
                float posZ = y * cellScale;

                MazeCell cell = Instantiate(
                    mazeCellPrefab,
                    new Vector3(posX, 0f, posZ),
                    Quaternion.identity,
                    root).GetComponent<MazeCell>();

                cell.transform.localScale *= cellScale;
                cell.name = $"{x}-{y}";
                cell.Setup(maze[x, y]);
            }
        }

        // スタートとゴールを自動配置
        SetStartAndGoal();
    }

    // 深さ優先探索による迷路生成
    private void GenerateMaze(int x, int y)
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

                    GenerateMaze(newX, newY);
                }
            }
        }
    }

    // 探索する方向をランダムに並べ替える
    private List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> ShuffleDirections()
    {
        List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> directions =
            new List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)>
        {
            (0, 1, MazeCellModel.Wall.Top, MazeCellModel.Wall.Bottom),
            (0, -1, MazeCellModel.Wall.Bottom, MazeCellModel.Wall.Top),
            (-1, 0, MazeCellModel.Wall.Left, MazeCellModel.Wall.Right),
            (1, 0, MazeCellModel.Wall.Right, MazeCellModel.Wall.Left)
        };

        for (int i = 0; i < directions.Count; i++)
        {
            var temp = directions[i];

            int randomIndex = random.Next(i, directions.Count);

            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        return directions;
    }

    // スタートとゴールの自動配置
    private void SetStartAndGoal()
    {
        // プレイヤーを左下に配置
        if (player != null)
        {
            player.position = new Vector3(
                0f,
                playerHeight,
                0f
            );
        }

        // ゴールを右上に配置
        if (goal != null)
        {
            goal.position = new Vector3(
                (width - 1) * cellScale,
                goalHeight,
                (height - 1) * cellScale
            );
        }
    }
}