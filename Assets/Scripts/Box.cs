using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private static float MOVE_TIME = 0.2f;       //1マス移動するのにかかる時間

    public MapStatus mapStatus;                  //現在のMapStatus.cs、MapDataの更新の際に参照
    public int boxPosX;                          //X座標
    public int boxPosY;                          //Y座標

    //Player.csで呼ばれる
    public void StartCorotineMoveBox(MapStatus.Direction dir)
    {
        StartCoroutine(MovingPosition(dir));
    }

    //箱の移動、MapDataの更新と座標の移動
    private IEnumerator MovingPosition(MapStatus.Direction dir)
    {
        mapStatus.UpdateMapData(boxPosX, boxPosY, null);

        float timer = 0;
        Vector3 moveDir = Vector3.zero;

        switch (dir)
        {
            case MapStatus.Direction.Left:
                boxPosX -= 1;
                moveDir = Vector3.left;
                break;
            case MapStatus.Direction.Up:
                boxPosY += 1;
                moveDir = Vector3.up;
                break;
            case MapStatus.Direction.Right:
                boxPosX += 1;
                moveDir = Vector3.right;
                break;
            case MapStatus.Direction.Down:
                boxPosY -= 1;
                moveDir = Vector3.down;
                break;
        }

        while (timer <= MOVE_TIME)
        {
            transform.position += moveDir / (MOVE_TIME / Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        mapStatus.UpdateMapData(boxPosX, boxPosY, transform.gameObject);
    }
}
