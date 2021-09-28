using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private static float MOVE_TIME = 0.2f;       //1�}�X�ړ�����̂ɂ����鎞��

    public MapStatus mapStatus;                  //���݂�MapStatus.cs�AMapData�̍X�V�̍ۂɎQ��
    public int boxPosX;                          //X���W
    public int boxPosY;                          //Y���W

    //Player.cs�ŌĂ΂��
    public void StartCorotineMoveBox(MapStatus.Direction dir)
    {
        StartCoroutine(MovingPosition(dir));
    }

    //���̈ړ��AMapData�̍X�V�ƍ��W�̈ړ�
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
