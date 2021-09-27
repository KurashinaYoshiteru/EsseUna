using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*
    public enum Direction
    {
        Left,
        Up,
        Right,
        Down,
        EnumMax
    }
    */

    //private MapStatus.Direction currentDir = MapStatus.Direction.Down;

    //キーコードをpublicでInspectorから取得、これを変えることで入力を差別化
    public KeyCode KEYCODE_MOVE_LEFT = KeyCode.LeftArrow;
    public KeyCode KEYCODE_MOVE_UP = KeyCode.UpArrow;
    public KeyCode KEYCODE_MOVE_RIGHT = KeyCode.RightArrow;
    public KeyCode KEYCODE_MOVE_DOWN = KeyCode.DownArrow;
    public KeyCode KEYCODE_ACTION = KeyCode.Space;

    public bool isActive;

    //private static float MOVE_INTERVAL = 0.1f;      //静止状態から動き始めるまでの時間

    private static int MOVE_FRAME = 60;             //1マス移動するのに必要なフレーム数  60だと1秒間で6マスくらい

    //private float firstInputTimer = 0;              //静止状態から動き出すまでの時間計測に使用
    //private float movingTimer = 0;                  //動いている最中の時間計測に使用
    private bool isMove = false;                    //動いている時true

    public MapStatus mapStatus;                     //マップステータス
    private SoundManager soundManager;

    public int playerPosX;                          //このプレイヤーのX座標
    public int playerPosY;                          //このプレイヤーのY座標

    // Start is called before the first frame update
    void Start()
    {
        if(transform.name == "MainPlayer")
        {
            isActive = true;
            SetColorAlpha(1.0f);
        }
        else if (transform.name == "SubPlayer")
        {
            isActive = false;
            SetColorAlpha(0.5f);
        }

        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchController();

        if (isActive)
        {
            Move();
        }

        /*
        //デバッグ用currentDir視覚化
        switch (currentDir)
        {
            case MapStatus.Direction.Left:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case MapStatus.Direction.Up:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case MapStatus.Direction.Right:
                transform.eulerAngles = new Vector3(0, 0, 270);
                break;
            case MapStatus.Direction.Down:
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
        }
        */
    }

    private void SwitchController()
    {
        if (Input.GetKeyDown(KEYCODE_ACTION))
        {
            if (isActive)
            {
                isActive = false;
                SetColorAlpha(0.5f);
                soundManager.playSound("switchControllerSE");
            }
            else
            {
                isActive = true;
                SetColorAlpha(1.0f);
            }
        }
    }

    private void SetColorAlpha(float alpha)
    {
        //Debug.Log("alpha");
        float r = transform.GetComponent<SpriteRenderer>().color.r;
        float g = transform.GetComponent<SpriteRenderer>().color.g;
        float b = transform.GetComponent<SpriteRenderer>().color.b;
        transform.GetComponent<SpriteRenderer>().color = new Color(r, g, b, alpha);
    }

    private void Move()
    {
        //静止状態のとき
        if (!isMove)
        {
            //キー入力と同時に向き変更
            if (Input.GetKeyDown(KEYCODE_MOVE_LEFT))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Left, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Left));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_UP))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Up, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Up));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_RIGHT))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Right, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Right));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_DOWN))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Down, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Down));
                }
            }
        }
    }


    IEnumerator MovingPosition(MapStatus.Direction dir)
    {
        mapStatus.UpdateMapData(playerPosX, playerPosY, null);

        switch (dir)
        {
            case MapStatus.Direction.Left:
                playerPosX -= 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.left / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Up:
                playerPosY += 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.up / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Right:
                playerPosX += 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.right / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Down:
                playerPosY -= 1;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.down / MOVE_FRAME;
                    yield return null;
                }
                break;
        }

        mapStatus.UpdateMapData(playerPosX, playerPosY, transform.gameObject);
        mapStatus.CheckGoal();
        mapStatus.CheckSwitch(playerPosX, playerPosY);
        isMove = false;
    }



    /*
    private void Move_()
    {
        //静止状態のとき
        if (!isMove)
        {
            //キー入力と同時に向き変更
            if (Input.GetKeyDown(KEYCODE_MOVE_LEFT))
            {
                //向いている方向と入力方向が同じなら移動
                if(currentDir == MapStatus.Direction.Left)
                {
                    StartCoroutine(MovingPosition(MapStatus.Direction.Left));
                }
                else
                {
                    currentDir = MapStatus.Direction.Left;
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_UP))
            {
                if (currentDir == MapStatus.Direction.Up)
                {
                    StartCoroutine(MovingPosition(MapStatus.Direction.Up));
                }
                else
                {
                    currentDir = MapStatus.Direction.Up;
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_RIGHT))
            {
                if (currentDir == MapStatus.Direction.Right)
                {
                    StartCoroutine(MovingPosition(MapStatus.Direction.Right));
                }
                else
                {
                    currentDir = MapStatus.Direction.Right;
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_DOWN))
            {
                if (currentDir == MapStatus.Direction.Down)
                {
                    StartCoroutine(MovingPosition(MapStatus.Direction.Down));
                }
                else
                {
                    currentDir = MapStatus.Direction.Down;
                }
            }

            //いずれかのキーが入力されているとき
            if (Input.GetKey(KEYCODE_MOVE_LEFT) || Input.GetKey(KEYCODE_MOVE_UP) ||
               Input.GetKey(KEYCODE_MOVE_RIGHT) || Input.GetKey(KEYCODE_MOVE_DOWN))
            {
                //入力開始からMOVE_INTERVAL時間経過したら動き出す
                firstInputTimer += Time.deltaTime;
                if (firstInputTimer > MOVE_INTERVAL)
                {
                    isMove = true;
                }
            }
            //途中で入力が終了したらタイマーリセット
            else
            {
                firstInputTimer = 0;
            }
        }
        //すでに動いているとき
        else if (isMove)
        {
            //時間を計測し動いている最中は処理終了
            movingTimer += Time.deltaTime;
            if (movingTimer >= MOVE_FRAME * Time.deltaTime)
            {
                movingTimer = 0;
            }
            else
            {
                return;
            }

            //入力方向に向きを変え移動
            if (Input.GetKey(KEYCODE_MOVE_LEFT))
            {
                StartCoroutine(MovingPosition(MapStatus.Direction.Left));
            }
            else if (Input.GetKey(KEYCODE_MOVE_UP))
            {
                StartCoroutine(MovingPosition(MapStatus.Direction.Up));
            }
            else if (Input.GetKey(KEYCODE_MOVE_RIGHT))
            {
                StartCoroutine(MovingPosition(MapStatus.Direction.Right));
            }
            else if (Input.GetKey(KEYCODE_MOVE_DOWN))
            {
                StartCoroutine(MovingPosition(MapStatus.Direction.Down));
            }
            //いずれのキーの入力もなければ動いている状態を終了させる
            else
            {
                isMove = false;
            }
        }
    }
    */

    /*
    IEnumerator MovingPosition(MapStatus.Direction dir)
    {

        yield return null;
        if (!mapStatus.CheckMovable(dir, playerPosX, playerPosY)) yield break;
        isMove = true;

        //CurrentMapStatus.UpdateMapData(MapStatus.FloorKind.Nomal, CurrentMapStatus.playerPosX, CurrentMapStatus.playerPosY);
        mapStatus.UpdateMapData(MapStatus.FloorKind.Nomal, playerPosX, playerPosY, null);

        switch (dir)
        {
            case MapStatus.Direction.Left:
                
                if(CheckBox(CurrentMapStatus.playerPosX - 1, CurrentMapStatus.playerPosY))
                {
                    CurrentMapStatus.mapObject[CurrentMapStatus.playerPosX - 1][CurrentMapStatus.playerPosY].GetComponent<Box>().test(MapStatus.Direction.Left);
                }
                CurrentMapStatus.playerPosX -= 1;
                
                if (CheckBox(dir, playerPosX, playerPosY))
                {
                    CurrentMapStatus.mapObject[playerPosX - 1][playerPosY].GetComponent<Box>().StartCorotineMoveBox(dir);
                }
                

                if (mapStatus.ReturnFloor(playerPosX - 1, playerPosY) == MapStatus.FloorKind.BoxOnNomal)
                {
                    if ((mapStatus.objectData[mapStatus.objectData[playerPosX - 1][playerPosY].transform.name].prefabName == CAN_PUSH_BOX) &&
                        (mapStatus.ReturnFloor(playerPosX - 2, playerPosY) == MapStatus.FloorKind.Nomal))
                    {
                        mapStatus.objectData[playerPosX - 1][playerPosY].GetComponent<Box>().StartCorotineMoveBox(dir);
                    }
                    else
                    {
                        break;
                    }
                }

                playerPosX -= 1;

                currentDir = MapStatus.Direction.Left;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.left / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Up:

                if (mapStatus.ReturnFloor(playerPosX, playerPosY + 1) == MapStatus.FloorKind.BoxOnNomal)
                {
                    if ((mapStatus.objectData[mapStatus.objectData[playerPosX][playerPosY + 1].transform.name].prefabName == CAN_PUSH_BOX) &&
                        (mapStatus.ReturnFloor(playerPosX, playerPosY + 2) == MapStatus.FloorKind.Nomal))
                    {
                        mapStatus.objectData[playerPosX][playerPosY + 1].GetComponent<Box>().StartCorotineMoveBox(dir);
                    }
                    else
                    {
                        break;
                    }
                }

                playerPosY += 1;

                currentDir = MapStatus.Direction.Up;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.up / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Right:

                if (mapStatus.ReturnFloor(playerPosX + 1, playerPosY) == MapStatus.FloorKind.BoxOnNomal)
                {
                    if ((mapStatus.objectData[mapStatus.objectData[playerPosX + 1][playerPosY].transform.name].prefabName == CAN_PUSH_BOX) &&
                     (mapStatus.ReturnFloor(playerPosX + 2, playerPosY) == MapStatus.FloorKind.Nomal))
                    {
                        mapStatus.objectData[playerPosX + 1][playerPosY].GetComponent<Box>().StartCorotineMoveBox(dir);
                    }
                    else
                    {
                        break;
                    }
                }

                playerPosX += 1;

                currentDir = MapStatus.Direction.Right;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.right / MOVE_FRAME;
                    yield return null;
                }
                break;

            case MapStatus.Direction.Down:

                if (mapStatus.ReturnFloor(playerPosX, playerPosY - 1) == MapStatus.FloorKind.BoxOnNomal)
                {
                    if ((mapStatus.objectData[mapStatus.objectData[playerPosX][playerPosY - 1].transform.name].prefabName == CAN_PUSH_BOX) &&
                        (mapStatus.ReturnFloor(playerPosX, playerPosY - 2) == MapStatus.FloorKind.Nomal))
                    {
                        mapStatus.objectData[playerPosX][playerPosY - 1].GetComponent<Box>().StartCorotineMoveBox(dir);
                    }
                    else
                    {
                        break;
                    }
                }

                playerPosY -= 1;

                currentDir = MapStatus.Direction.Down;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.down / MOVE_FRAME;
                    yield return null;
                }
                break;
        }

        //CurrentMapStatus.UpdateMapData(MapStatus.FloorKind.PlayerOnNomal, CurrentMapStatus.playerPosX, CurrentMapStatus.playerPosY);
        mapStatus.UpdateMapData(MapStatus.FloorKind.PlayerOnNomal, playerPosX, playerPosY, transform.gameObject);

    }
    */



    /*
    private void PushBox(MapStatus.Direction dir, int posX, int posY)
    {
        switch (dir)
        {
            case MapStatus.Direction.Left:
                if (CurrentMapStatus.ReturnFloor(posX - 1, posY) == MapStatus.FloorKind.BoxOnNomal)
                {
                    if (CurrentMapStatus.ReturnFloor(posX - 2, posY) == MapStatus.FloorKind.Nomal)
                    {
                        CurrentMapStatus.mapObject[playerPosX - 1][playerPosY].GetComponent<Box>().StartCorotineMoveBox(dir); 
                        
                    }
                    else
                    {
                        break;
                    }
                }

                playerPosX -= 1;

                currentDir = MapStatus.Direction.Left;
                for (int i = 0; i < MOVE_FRAME; i++)
                {
                    transform.position += Vector3.left / MOVE_FRAME;
                    yield return null;
                }
                break;
        }

    }
    
    private bool CheckBox(MapStatus.Direction dir, int posX, int posY)
    {
        MapStatus.FloorKind fk = MapStatus.FloorKind.Nomal;

        switch (dir)
        {
            case MapStatus.Direction.Left:
                fk = CurrentMapStatus.ReturnFloor(posX - 1, posY);
                break;
            case MapStatus.Direction.Up:
                fk = CurrentMapStatus.ReturnFloor(posX, posY + 1);
                break;
            case MapStatus.Direction.Right:
                fk = CurrentMapStatus.ReturnFloor(posX + 1, posY);
                break;
            case MapStatus.Direction.Down:
                fk = CurrentMapStatus.ReturnFloor(posX, posY - 1);
                break;
        }

        if(fk == MapStatus.FloorKind.BoxOnNomal)
        {
            return true;
        }

        return false;
    }
    
    */

}
