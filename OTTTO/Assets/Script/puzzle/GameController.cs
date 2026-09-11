using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameContollore : MonoBehaviour
{
    public GameObject PieceBase;

    public Sprite[] PieceFaces;

    private List<GameObject> PieceList = new List<GameObject>();

    public Vector3[] Pos;

    public GameObject ClearMessage;





    private int[,] puzzle1 = new int[3, 3]

    {

        {0,1,2 },

        {3,4,5 },

        {6,8,7 },

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

                {

                    continue;

                }

                PieceList[puzzle1[i, j]].transform.position = new Vector2(j, i * offsetY);

            }

        }

    }

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

            //ƒNƒŠƒA‚µ‚½‚Æ‚«‚Ì‰‰o
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

        SceneManager.LoadScene("stage5");

        
        this.transform.position = new Vector3(48f, 1f, 44f);
        Debug.Log("stage5");
    }


}
