using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //キーコードを変数化して後で変えられるようにしておく
    private KeyCode KEYCODE_MOVE_LEFT = KeyCode.LeftArrow;
    private KeyCode KEYCODE_MOVE_UP = KeyCode.UpArrow;
    private KeyCode KEYCODE_MOVE_RIGHT = KeyCode.RightArrow;
    private KeyCode KEYCODE_MOVE_DOWN = KeyCode.DownArrow;
    private KeyCode KEYCODE_ACTION = KeyCode.Space;
    private static float MOVE_TIME = 0.2f;       //1マス移動するのにかかる時間

    private bool isMove = false;                 //動いている時true
    private bool isActive;                       //操作中のときはtrue
    private SoundManager soundManager;           //サウンドマネージャー、アクティブプレイヤー切り替え時のSE再生に使用

    public MapStatus mapStatus;                  //マップステータス、プレイヤー生成時に渡される
    public int playerPosX;                       //このプレイヤーのX座標
    public int playerPosY;                       //このプレイヤーのY座標

    // Start is called before the first frame update
    void Start()
    {
        //操作プレイヤー初期化
        SetController();

        //soundManagerの初期化
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //操作プレイヤー切り替え
        SwitchController();

        //操作中のときは移動処理
        if (isActive)
        {
            Move();
        }
    }

    //操作プレイヤー初期化
    private void SetController()
    {
        //MainPlayerをアクティブにする
        if (transform.name == "MainPlayer")
        {
            isActive = true;
            SetColorAlpha(1.0f);
        }
        //SubPlayerを非アクティブにする
        else if (transform.name == "SubPlayer")
        {
            isActive = false;
            SetColorAlpha(0.5f);
        }
    }

    //操作プレイヤー切り替え
    private void SwitchController()
    {
        if (Input.GetKeyDown(KEYCODE_ACTION))
        {
            if (isActive)
            {
                isActive = false;
                SetColorAlpha(0.5f);
                soundManager.PlaySound("switchControllerSE");
            }
            else
            {
                isActive = true;
                SetColorAlpha(1.0f);
            }
        }
    }

    //操作プレイヤー切り替え時に色を変える
    private void SetColorAlpha(float alpha)
    {
        float r = transform.GetComponent<SpriteRenderer>().color.r;
        float g = transform.GetComponent<SpriteRenderer>().color.g;
        float b = transform.GetComponent<SpriteRenderer>().color.b;
        transform.GetComponent<SpriteRenderer>().color = new Color(r, g, b, alpha);
    }

    //移動処理
    private void Move()
    {
        //動いていないときに入力を受け付ける
        if (!isMove)
        {
            //移動キー押し込み時に動く
            if (Input.GetKeyDown(KEYCODE_MOVE_LEFT))
            {
                //入力方向に移動できるか確認
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

    //MapDataの更新と座標移動、コルーチンで他の入力をスルーさせる
    IEnumerator MovingPosition(MapStatus.Direction dir)
    {
        //もともといた位置をnullに
        mapStatus.UpdateMapData(playerPosX, playerPosY, null);

        //一時的な変数宣言
        float timer = 0;
        Vector3 moveDir = Vector3.zero;

        //入力方向に応じてplayerPosの更新と位置の移動
        switch (dir)
        {
            case MapStatus.Direction.Left:
                //playerPosの更新
                playerPosX -= 1;
                //位置の移動
                moveDir = Vector3.left;
                break;

            case MapStatus.Direction.Up:
                playerPosY += 1;
                moveDir = Vector3.up;
                break;

            case MapStatus.Direction.Right:
                playerPosX += 1;
                moveDir = Vector3.right;
                break;

            case MapStatus.Direction.Down:
                playerPosY -= 1;
                moveDir = Vector3.down;
                break;
        }

        while (timer <= MOVE_TIME)
        {
            transform.position += moveDir / (MOVE_TIME / Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        //新しく移動した位置をこのオブジェクトに
        mapStatus.UpdateMapData(playerPosX, playerPosY, transform.gameObject);

        //ゴールチェックとスイッチチェック
        mapStatus.CheckGoal();
        mapStatus.CheckSwitch(playerPosX, playerPosY);

        //移動終了
        isMove = false;
    }

}
