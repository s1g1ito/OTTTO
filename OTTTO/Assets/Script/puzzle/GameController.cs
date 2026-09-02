using System.Collections.Generic;
using UnityEngine;

public class GameContollore : MonoBehaviour
{
    public GameObject PieceBase;

    public Sprite[] PieceFaces;

    private List<GameObject> PieceList = new List<GameObject>();

   

    //

    private int[,] puzzle1 = new int[3, 3]

    {

        {2,6,0 },

        {7,8,5 },

        {1,3,4 },

    };


    void Start()

    {

        CreatePieces();

    }
    void CreatePieces()

    {

        for (int i = 0; i < 8; i++)

        {

            var piece = Instantiate(PieceBase);

            piece.GetComponent<SpriteRenderer>().sprite = PieceFaces[i];

            PieceList.Add(piece);

        }

        Dealing();

    }

    void Dealing()

    {

        float offsetY = -1.3f;



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


}
