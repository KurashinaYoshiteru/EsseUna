using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{

    private static float MOVE_INTERVAL = 0.1f;      //�Î~��Ԃ��瓮���n�߂�܂ł̎���
    private static int MOVE_FRAME = 45;             //1�}�X�ړ�����̂ɕK�v�ȃt���[����  60����1�b�Ԃ�6�}�X���炢


    public MapStatus mapStatus;

    public int boxPosX;
    public int boxPosY;

    private bool isMove = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCorotineMoveBox(MapStatus.Direction dir)
    {
        StartCoroutine(MovingPosition(dir));
    }

    public IEnumerator MovingPosition(MapStatus.Direction dir)
    {
        mapStatus.UpdateMapData(boxPosX, boxPosY, null);

        switch (dir)
        {
            case MapStatus.Direction.Left:
                boxPosX -= 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.left / MOVE_FRAME;
                    yield return null;
                }
                break;
            case MapStatus.Direction.Up:
                boxPosY += 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.up / MOVE_FRAME;
                    yield return null;
                }
                break;
            case MapStatus.Direction.Right:
                boxPosX += 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.right / MOVE_FRAME;
                    yield return null;
                }
                break;
            case MapStatus.Direction.Down:
                boxPosY -= 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.down / MOVE_FRAME;
                    yield return null;
                }
                break;
        }
        mapStatus.UpdateMapData(boxPosX, boxPosY, transform.gameObject);

    }
}
