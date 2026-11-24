
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameContollore : MonoBehaviour
{
    public GameObject PieceBase;
    public Sprite[] PieceFaces;
    public Vector3[] Pos;
    public GameObject ClearMessage;

    private List<GameObject> PieceList = new List<GameObject>();

    // ヒント機能
    private GameObject hintPiece;
    private Color originalColor;
    private float hintDuration = 2f;

    private int[,] puzzle1 = new int[3, 3]
    {
        { 6, 2, 5 },
        { 7, 1, 4 },
        { 3, 0, 8 }
    };

    void Start()
    {
        ClearMessage.SetActive(false);
        SetCorrectPos();
        CreatePieces();
    }

    void SetCorrectPos()
    {
        int n = 0;
        float offsetY = -1f;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Pos[n] = new Vector2(j, i * offsetY);
                n++;
            }
        }
    }

    void CreatePieces()
    {
        for (int i = 0; i < 9; i++)
        {
            var piece = Instantiate(PieceBase);

            piece.GetComponent<SpriteRenderer>().sprite = PieceFaces[i];

            PieceList.Add(piece);
        }

        // 8番を空白マスとして非表示
        PieceList[8].SetActive(false);

        Dealing();
    }

    void Dealing()
    {
        float offsetY = -1f;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (puzzle1[i, j] == 8)
                    continue;

                PieceList[puzzle1[i, j]].transform.position =
                    new Vector2(j, i * offsetY);
            }
        }
    }


    // ヒント機能
    public void ShowHint()
    {
        ClearHint();

        if (ClearMessage.activeSelf)
            return;

        string currentState = GetCurrentState();

        if (currentState == null)
        {
            Debug.Log("パネルの移動が完了してからヒントを押してください");
            return;
        }

        int tileIndex = FindHintPiece(currentState);

        if (tileIndex == -1)
        {
            Debug.Log("ヒントが見つかりません");
            return;
        }

        hintPiece = PieceList[tileIndex];

        SpriteRenderer sr =
            hintPiece.GetComponent<SpriteRenderer>();

        originalColor = sr.color;

        // 赤色に光らせる
        sr.color = Color.red;

        Invoke(nameof(ClearHint), hintDuration);

        Debug.Log("次に動かすパネル：" + tileIndex);
    }

    // 現在の盤面を取得
    string GetCurrentState()
    {
        char[] board = new char[9];

        for (int i = 0; i < 9; i++)
        {
            board[i] = '8';
        }

        for (int i = 0; i < 8; i++)
        {
            float minDistance = float.MaxValue;
            int positionIndex = -1;

            for (int j = 0; j < 9; j++)
            {
                float distance = Vector2.Distance(
                    PieceList[i].transform.position,
                    Pos[j]
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    positionIndex = j;
                }
            }

            if (minDistance > 0.2f || positionIndex == -1)
                return null;

            if (board[positionIndex] != '8')
                return null;

            board[positionIndex] = (char)('0' + i);
        }

        return new string(board);
    }

    // A*探索用クラス
    class SearchNode
    {
        public string state;
        public int g;
        public int f;
        public int order;

        public SearchNode(string s, int cost, int score, int number)
        {
            state = s;
            g = cost;
            f = score;
            order = number;
        }
    }

    // A*アルゴリズム
    int FindHintPiece(string start)
    {
        string goal = "012345678";

        if (start == goal)
            return -1;

        var open = new SortedSet<SearchNode>(
            Comparer<SearchNode>.Create((a, b) =>
            {
                int result = a.f.CompareTo(b.f);

                if (result != 0)
                    return result;

                return a.order.CompareTo(b.order);
            })
        );

        Dictionary<string, int> bestG =
            new Dictionary<string, int>();

        Dictionary<string, string> cameFrom =
            new Dictionary<string, string>();

        int order = 0;

        bestG[start] = 0;

        open.Add(new SearchNode(
            start, 0, ManhattanDistance(start), order++
        ));

        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };

        while (open.Count > 0)
        {
            SearchNode current = open.Min;
            open.Remove(current);

            if (bestG[current.state] != current.g)
                continue;

            if (current.state == goal)
            {
                string firstStep = current.state;

                while (cameFrom[firstStep] != start)
                {
                    firstStep = cameFrom[firstStep];
                }

                for (int i = 0; i < 8; i++)
                {
                    char tile = (char)('0' + i);

                    if (start.IndexOf(tile) !=
                        firstStep.IndexOf(tile))
                    {
                        return i;
                    }
                }

                return -1;
            }

            int blank = current.state.IndexOf('8');

            int row = blank / 3;
            int col = blank % 3;

            for (int d = 0; d < 4; d++)
            {
                int nr = row + dr[d];
                int nc = col + dc[d];

                if (nr < 0 || nr >= 3 ||
                    nc < 0 || nc >= 3)
                    continue;

                int nextIndex = nr * 3 + nc;

                char[] nextBoard =
                    current.state.ToCharArray();

                char temp = nextBoard[blank];

                nextBoard[blank] = nextBoard[nextIndex];
                nextBoard[nextIndex] = temp;

                string nextState = new string(nextBoard);

                int newG = current.g + 1;

                if (!bestG.ContainsKey(nextState) ||
                    newG < bestG[nextState])
                {
                    bestG[nextState] = newG;
                    cameFrom[nextState] = current.state;

                    int h = ManhattanDistance(nextState);

                    open.Add(new SearchNode(
                        nextState,
                        newG,
                        newG + h,
                        order++
                    ));
                }
            }
        }

        return -1;
    }

    // マンハッタン距離
    int ManhattanDistance(string state)
    {
        int distance = 0;

        for (int i = 0; i < 8; i++)
        {
            int position = state.IndexOf((char)('0' + i));

            int currentRow = position / 3;
            int currentCol = position % 3;

            int goalRow = i / 3;
            int goalCol = i % 3;

            distance += Mathf.Abs(currentRow - goalRow);
            distance += Mathf.Abs(currentCol - goalCol);
        }

        return distance;
    }

    // ヒント表示を解除
    public void ClearHint()
    {
        CancelInvoke(nameof(ClearHint));

        if (hintPiece != null)
        {
            SpriteRenderer sr =
                hintPiece.GetComponent<SpriteRenderer>();

            sr.color = originalColor;

            hintPiece = null;
        }
    }


    // クリア判定
    public void ClearCheck()
    {
        if (PieceList[0].transform.position == Pos[0]
            && PieceList[1].transform.position == Pos[1]
            && PieceList[2].transform.position == Pos[2]
            && PieceList[3].transform.position == Pos[3]
            && PieceList[4].transform.position == Pos[4]
            && PieceList[5].transform.position == Pos[5]
            && PieceList[6].transform.position == Pos[6]
            && PieceList[7].transform.position == Pos[7])
        {
            ClearHint();

            PieceList[8].SetActive(true);
            PieceList[8].transform.position = new Vector2(2, -2f);

            foreach (var item in PieceList)
            {
                item.GetComponent<PieceMove>().isClear = true;
            }

            ClearMessage.SetActive(true);

            Invoke("LastPiece", 1f);
        }
    }

    void LastPiece()
    {
        Debug.Log("last");

        PieceList[8].SetActive(true);
        PieceList[8].transform.position = new Vector2(2, -2f);

        FogOfWar.RequestMapLoad("Stage5");
        SceneManager.LoadScene("Stage5");

        this.transform.position = new Vector3(46f, 1f, 45f);

        Debug.Log("stage5");
    }
}
