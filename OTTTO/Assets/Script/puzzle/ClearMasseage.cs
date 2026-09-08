using UnityEngine;

public class ClearMasseage : MonoBehaviour
{
    //初期位置、止まる位置、移動スピードを計算する変数を定義

    private Vector2 StartPos;

    private Vector2 EndPos;

    private float step = 0;



    void Start()

    {

        //初期位置と止まる位置を指定

        StartPos = transform.position;

        EndPos = new Vector2(1, 1f);

    }



    void Update()

    {

        if (step > 1)

        {

            return;

        }

        step += Time.deltaTime;

        transform.position = Vector2.Lerp(StartPos, EndPos, step / 1);

    }
}
