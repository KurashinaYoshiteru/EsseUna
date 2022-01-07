using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    private static int unlockedStageNum;                     //解放ステージ数0～7
    public GameObject[] stageLockUI = new GameObject[7];     //ステージ2～ステージ8まで、計７個の蓋絵

    private void Start()
    {
        //GameManagerからステージクリア数を取得
        GameObject gameManagerObject = GameObject.Find("GameManager");
        unlockedStageNum = gameManagerObject.GetComponent<GameManager>().stageClearNum;

        //解放ステージ数以下の蓋絵をオフにする
        for (int i = 0; i < unlockedStageNum; i++)
        {
            stageLockUI[i].SetActive(false);
        }
    }

}
