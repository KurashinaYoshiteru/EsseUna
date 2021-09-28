using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TMPro;

public class MapStatus : MonoBehaviour
{
    private MapDatas mapData;                    //マップの各情報が入ったデータ
    private GameObject mapTileImageParent;       //床のマップチップの親
    private GameObject objectParent;             //オブジェクトの親
    private GameObject gameManagerObject;        //ゲームマネージャーオブジェクト
    private SoundManager soundManager;           //サウンドマネージャーコンポーネント

    //各オブジェクトのプレハブ
    public GameObject mainPlayerPrefab;
    public GameObject subPlayerPrefab;
    public GameObject mainBoxPrefab;
    public GameObject subBoxPrefab;
    public GameObject boxPrefab;

    public int remainTurn;                    //残りターン数
    public int stageNum;                      //ステージ数

    //床の種類
    public enum FloorKind
    {
        Nomal,              //普通の床
        Immovable,          //動けない床
        MainPlayerOnly,     //メインプレイヤーが踏める床
        SubPlayerOnly,      //サブプレイヤーが踏める床
        PlayerSwitch,       //プレイヤーが入れ替わるスイッチ
        BoxSwitch,          //箱が入れ替わるスイッチ
        ClearBoxSwitch,     //箱が消えるスイッチ
        Goal                //ゴール
    }

    //オブジェクトの進む方向、PlayerとBoxで使う
    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    public FloorKind[][] floorData;         //常に変わることのない床の種類のデータ
    public GameObject[][] objectData;       //常に更新されるマップ上のオブジェクト(プレイヤーや箱)などのデータ

    private int goalPosX = 0;
    private int goalPosY = 0;

    private GameObject mainPlayer;                              //メインプレイヤー
    private GameObject subPlayer;                               //サブプレイヤー
    private List<GameObject> box = new List<GameObject>();      //箱のリスト
    public GameObject mainCamera;                               //カメラ
    public float cameraBlank = 0;                               //カメラの余白

    //UIを宣言
    private GameObject stageClearUI;
    private GameObject gameOverUI;
    private GameObject backStageSelectButtonUI;
    private GameObject resetButtonUI;
    private TextMeshProUGUI stageNameText;
    private TextMeshProUGUI remainTurnText;


    // Start is called before the first frame update
    void Start()
    {
        //gameManagerObjectとsoundManager初期化
        gameManagerObject = GameObject.Find("GameManager");
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //マップチップ生成
        StartCoroutine(CreateMapchips());

        //プレイヤーや箱生成
        CreateObjects();

        //床のデータを初期化
        InitializeFloorData();

        //カメラのセット
        SetCamera();
    }


    //マップチップ生成
    private IEnumerator CreateMapchips()
    {
        //マップチップの親を指定
        if (GameObject.Find("MapTileImageParent"))
        {
            mapTileImageParent = GameObject.Find("MapTileImageParent");
        }

        //一つずつマップチップを生成、Addressableでマップチップ名に一致する画像を生成
        for (int i = 0; i < mapData.mapTile.Count; i++)
        {
            GameObject tmpParent = new GameObject("MapTile (" + (i + 1) + ")");
            tmpParent.transform.parent = mapTileImageParent.transform;
            tmpParent.transform.localPosition = new Vector2(0, mapData.mapTile.Count - 1 - i);

            for(int j = 0; j < mapData.mapTile[i].Length; j++)
            {
                bool isImage = false;       //対応する画像があればtrue

                //対応するマップチップを検索して生成していく
                foreach(string key in mapData.mapTileImageData.Keys)
                {
                    if(mapData.mapTile[i][j] == key)
                    {
                        string imageAddress = "";
                        mapData.mapTileImageData.TryGetValue(key, out imageAddress);

                        var tmpObj = Addressables.LoadAssetAsync<GameObject>("Assets/Mapchips/" + imageAddress + ".prefab");
                        yield return tmpObj;

                        GameObject tmpGameObj = Instantiate(tmpObj.Result, tmpParent.transform);
                        tmpGameObj.transform.localPosition = new Vector2(j, 0);

                        isImage = true;
                        break;
                    }
                }

                if (!isImage)
                {
                    Debug.LogError(mapData.mapTile[i][j] + "に対応する画像はありませんでした。");
                }
            }
        }

        //生成完了したらGameManagerにこれを伝える
        gameManagerObject.GetComponent<GameManager>().createMapchipsCompleted = true;
    }

    //オブジェクト生成
    private void CreateObjects()
    {
        //mapObjectDataの初期化
        objectData = new GameObject[mapData.mapTile[0].Length][];
        for (int i = 0; i < mapData.mapTile[0].Length; i++)
        {
            objectData[i] = new GameObject[mapData.mapTile.Count];
            for (int j = 0; j < mapData.mapTile.Count; j++)
            {
                objectData[i][j] = null;
            }
        }

        //プレイヤーの親を指定
        if (GameObject.Find("ObjectParent"))
        {
            objectParent = GameObject.Find("ObjectParent");
        }

        //一つずつオブジェクトを生成
        foreach (string key in mapData.objectData.Keys)
        {
            //一時的に使う変数宣言、初期化
            ObjectDatas tmpData = mapData.objectData[key];
            string tmpPrefabName = tmpData.prefabName;
            int tmpPosX = tmpData.posX;
            int tmpPosY = tmpData.posY;

            GameObject tmpObj = null;

            //指定したプレハブ名によって生成するオブジェクトを変える
            switch (tmpPrefabName)
            {
                case "MainPlayer":
                    tmpObj = mainPlayerPrefab;
                    break;
                case "SubPlayer":
                    tmpObj = subPlayerPrefab;
                    break;
                case "Box":
                    tmpObj = boxPrefab;
                    break;
                case "MainBox":
                    tmpObj = mainBoxPrefab;
                    break;
                case "SubBox":
                    tmpObj = subBoxPrefab;
                    break;
                default:
                    break;
            }

            //オブジェクト生成等
            GameObject tmpGameObj = Instantiate(tmpObj, objectParent.transform);
            tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
            tmpGameObj.transform.name = key;

            //生成したオブジェクトがプレイヤーか箱か判別
            if (tmpGameObj.tag == "MainPlayer" || tmpGameObj.tag == "SubPlayer")
            {
                //Player.csに情報を渡す
                tmpGameObj.GetComponent<Player>().mapStatus = this;
                tmpGameObj.GetComponent<Player>().playerPosX = tmpPosX;
                tmpGameObj.GetComponent<Player>().playerPosY = tmpPosY;

                if(tmpGameObj.tag == "MainPlayer")
                {
                    mainPlayer = tmpGameObj;
                }
                else if (tmpGameObj.tag == "SubPlayer")
                {
                    subPlayer = tmpGameObj;
                }

            }
            else if (tmpGameObj.tag == "MainBox" || tmpGameObj.tag == "SubBox" || tmpGameObj.tag == "Box")
            {
                //Box.csに情報を渡す
                tmpGameObj.GetComponent<Box>().mapStatus = this;
                tmpGameObj.GetComponent<Box>().boxPosX = tmpPosX;
                tmpGameObj.GetComponent<Box>().boxPosY = tmpPosY;

                box.Add(tmpGameObj);
            }

            //各座標の位置にオブジェクトを入れる
            objectData[tmpPosX][tmpPosY] = tmpGameObj;
        }
    }


    //mapDataに各データを保存する、MapReader.csで使用
    public void SetMapData(MapDatas md)
    {
        mapData = md;

        mapData.limitPosXmin = (int)transform.position.x;
        mapData.limitPosXmax = mapData.limitPosXmin + mapData.mapTile[0].Length - 1;
        mapData.limitPosYmin = (int)transform.position.y;
        mapData.limitPosYmax = mapData.limitPosYmax + mapData.mapTile.Count - 1;
    }

    //floorDataの初期化
    private void InitializeFloorData()
    {
        floorData = new FloorKind[mapData.mapTile[0].Length][];
        for (int i = 0; i < mapData.mapTile[0].Length; i++)
        {
            floorData[i] = new FloorKind[mapData.mapTile.Count];
        }

        //mapDataの記号ごとにどの種類の床か決める
        for (int i = 0; i < mapData.mapTile.Count; i++)
        {
            for (int j = 0; j < mapData.mapTile[i].Length; j++)
            {
                switch (mapData.mapTile[mapData.mapTile.Count - 1 - i][j])
                {
                    case "0":
                        floorData[j][i] = FloorKind.Nomal;
                        break;
                    case "1":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "2":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "3":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "4":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "5":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "6":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "7":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "8":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "9":
                        floorData[j][i] = FloorKind.Immovable;
                        break;
                    case "M":
                        floorData[j][i] = FloorKind.MainPlayerOnly;
                        break;
                    case "S":
                        floorData[j][i] = FloorKind.SubPlayerOnly;
                        break;
                    case "P":
                        floorData[j][i] = FloorKind.PlayerSwitch;
                        break;
                    case "B":
                        floorData[j][i] = FloorKind.BoxSwitch;
                        break;
                    case "C":
                        floorData[j][i] = FloorKind.ClearBoxSwitch;
                        break;
                    case "G":
                        floorData[j][i] = FloorKind.Goal;
                        goalPosX = j;
                        goalPosY = i;
                        break;
                    default:
                        break;
                }
            }
        }

        //UI取得と非表示にするものは非表示に
        GetUIinGameScene();
        stageClearUI.SetActive(false);
        gameOverUI.SetActive(false);

        //残りターンやステージ数をmapDataから取得
        remainTurn = mapData.movableTime;
        stageNum = mapData.stageNum;

        //これらをUIに渡す
        SetRemainTurn();
        SetStageNum();

    }

    //objectDataを更新する
    public void UpdateMapData(int posX, int posY, GameObject gameObject)
    {
        objectData[posX][posY] = gameObject;
    }

    //targetが座標(posX, posY)からdirの方向に動けるか確認する、動けるならtrue
    public bool CheckMovable(GameObject target, Direction dir, int posX, int posY)
    {
        bool returnValue = false;              //返り値、動けるときtrue
        string targetTag = target.tag;         //ターゲットのタグ
        int checkPosX = posX;                  //確認するX座標
        int checkPosY = posY;                  //確認するY座標

        switch (dir)
        {
            case Direction.Left:
                checkPosX -= 1;
                break;
            case Direction.Up:
                checkPosY += 1;
                break;
            case Direction.Right:
                checkPosX += 1;
                break;
            case Direction.Down:
                checkPosY -= 1;
                break;
        }

        FloorKind returnFloor = ReturnFloor(checkPosX, checkPosY);

        //各オブジェクトで動ける条件を満たしていればtrue
        switch (targetTag)
        {
            case "MainPlayer":
                //まずは床の種類(floorData)を見る
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.PlayerSwitch
                    || returnFloor == FloorKind.BoxSwitch || returnFloor == FloorKind.ClearBoxSwitch || returnFloor == FloorKind.Goal)
                {
                    //次にオブジェクトがあるか(objectData)を見る
                    if(objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.PlaySound("mainPlayerMoveSE");
                    }
                    //プレイヤーの場合、押せる箱であるか見る
                    else if (objectData[checkPosX][checkPosY].tag == "MainBox" || objectData[checkPosX][checkPosY].tag == "Box")
                    {
                        //押せる箱ならその箱は位置的に押せる箱かを見る
                        if (CheckMovable(objectData[checkPosX][checkPosY], dir, checkPosX, checkPosY))
                        {
                            returnValue = true;
                            soundManager.PlaySound("mainPlayerMoveSE");
                            objectData[checkPosX][checkPosY].GetComponent<Box>().StartCorotineMoveBox(dir);
                        }
                    }
                }
                break;
            case "SubPlayer":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.SubPlayerOnly || returnFloor == FloorKind.PlayerSwitch
                    || returnFloor == FloorKind.BoxSwitch || returnFloor == FloorKind.ClearBoxSwitch)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.PlaySound("subPlayerMoveSE");
                    }
                    else if (objectData[checkPosX][checkPosY].tag == "SubBox" || objectData[checkPosX][checkPosY].tag == "Box")
                    {
                        if (CheckMovable(objectData[checkPosX][checkPosY], dir, checkPosX, checkPosY))
                        {
                            returnValue = true;
                            soundManager.PlaySound("subPlayerMoveSE");
                            objectData[checkPosX][checkPosY].GetComponent<Box>().StartCorotineMoveBox(dir);
                        }
                    }
                }
                break;
            case "Box":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.SubPlayerOnly)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.PlaySound("boxMoveSE");
                    }
                }
                break;
            case "MainBox":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.SubPlayerOnly)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.PlaySound("boxMoveSE");
                    }
                }
                break;
            case "SubBox":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.SubPlayerOnly)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.PlaySound("boxMoveSE");
                    }
                }
                break;
            default:
                Debug.LogError("オブジェクトのタグが指定されていません");
                break;
        }

        return returnValue;
    }

    //任意の場所の床の種類を返す
    public FloorKind ReturnFloor(int posX, int posY)
    {
        if ((posX < mapData.limitPosXmin) || (posX > mapData.limitPosXmax)
        || (posY < mapData.limitPosYmin) || (posY > mapData.limitPosYmax))
        {
            return FloorKind.Immovable;
        }

        return floorData[posX][posY];
    }

    //マップのサイズによってカメラの位置を設定する
    private void SetCamera()
    {
        float sizeX = mapData.limitPosXmax - mapData.limitPosXmin;
        float sizeY = mapData.limitPosYmax - mapData.limitPosYmin;
        float centerX = sizeX / 2;
        float centerY = sizeY / 2;
        float blank = cameraBlank;
        mainCamera.GetComponent<Camera>().transform.position = new Vector3(centerX, centerY, 0);

        //幅の広い方に合わせてカメラの画角を広げる
        if (centerX * 9.0f / 16.0f > centerY)
        {
            mainCamera.GetComponent<Camera>().orthographicSize = (centerX + blank) * 9.0f / 16.0f;
        }
        else
        {
            mainCamera.GetComponent<Camera>().orthographicSize = centerY + blank;
        }
    }


    //ゴールチェックとついでにターン数チェックも、プレイヤーが移動するたびに呼ばれる
    public void CheckGoal()
    {
        //ゴールチェック
        if (objectData[goalPosX][goalPosY] != null)
        {
            if (objectData[goalPosX][goalPosY].tag == "MainPlayer")
            {
                //Debug.Log("STAGE CLEAR!!");
                Goal();
                return;
            }
        }

        //ターン数チェック
        remainTurn--;
        SetRemainTurn();
        if (remainTurn <= 0)
        {
            GameOver();
        }
    }

    //ゴール時の処理
    void Goal()
    {
        RemovePlayerComponent();
        SetActiveStageClearText();
        soundManager.PlaySound("stageClearSE");
    }

    //ゲームオーバー時の処理
    void GameOver()
    {
        RemovePlayerComponent();
        SetActiveGameOverText();
        soundManager.PlaySound("gameOverSE");
    }

    //GoalかGameOver後、プレイヤー動けないようにする
    private void RemovePlayerComponent()
    {
        mainPlayer.GetComponent<Player>().enabled = false;
        subPlayer.GetComponent<Player>().enabled = false;
    }

    //プレイヤーの踏んでいる床にスイッチがあれば該当スイッチの処理を行う
    public void CheckSwitch(int posX, int posY)
    {
        FloorKind checkFloor = floorData[posX][posY];
        switch (checkFloor)
        {
            case FloorKind.PlayerSwitch:
                OnPlayerSwitch();
                break;
            case FloorKind.BoxSwitch:
                OnBoxSwitch();
                break;
            case FloorKind.ClearBoxSwitch:
                break;
        }
    }

    //プレイヤーの位置が入れ替わるスイッチ
    public void OnPlayerSwitch()
    {
        int tmpMainPosX = mainPlayer.GetComponent<Player>().playerPosX;
        int tmpMainPosY = mainPlayer.GetComponent<Player>().playerPosY;
        int tmpSubPosX = subPlayer.GetComponent<Player>().playerPosX;
        int tmpSubPosY = subPlayer.GetComponent<Player>().playerPosY;

        mainPlayer.GetComponent<Player>().playerPosX = tmpSubPosX;
        mainPlayer.GetComponent<Player>().playerPosY = tmpSubPosY;
        mainPlayer.transform.localPosition = new Vector2(tmpSubPosX, tmpSubPosY);
        UpdateMapData(tmpSubPosX, tmpSubPosY, mainPlayer);

        subPlayer.GetComponent<Player>().playerPosX = tmpMainPosX;
        subPlayer.GetComponent<Player>().playerPosY = tmpMainPosY;
        subPlayer.transform.localPosition = new Vector2(tmpMainPosX, tmpMainPosY);
        UpdateMapData(tmpSubPosX, tmpSubPosY, subPlayer);

        soundManager.PlaySound("playerSwitchSE");
    }

    //箱の種類が変わるスイッチ、一度Destroyしてから生成している
    public void OnBoxSwitch()
    {
        for(int i = 0; i < box.Count; i++)
        {
            GameObject boxObject = box[i];
            string boxTag = boxObject.tag;

            if (boxTag == "Box")
            {
                continue;
            }

            GameObject newBoxObject = null;
            int tmpBoxPosX = boxObject.GetComponent<Box>().boxPosX;
            int tmpBoxPosY = boxObject.GetComponent<Box>().boxPosY;
            string tmpBoxName = boxObject.name;

            Destroy(boxObject);

            if (boxTag == "MainBox")
            {
                newBoxObject = Instantiate(subBoxPrefab, objectParent.transform);
            }
            else if (boxTag == "SubBox")
            {
                newBoxObject = Instantiate(mainBoxPrefab, objectParent.transform);
            }
            else
            {
                Debug.LogError("タグ関連でエラーが発生しています");
            }

            newBoxObject.transform.name = tmpBoxName;
            newBoxObject.GetComponent<Box>().mapStatus = this;
            newBoxObject.GetComponent<Box>().boxPosX = tmpBoxPosX;
            newBoxObject.GetComponent<Box>().boxPosY = tmpBoxPosY;
            newBoxObject.transform.localPosition = new Vector2(tmpBoxPosX, tmpBoxPosY);
            UpdateMapData(tmpBoxPosX, tmpBoxPosY, newBoxObject);
            box[i] = newBoxObject;
        }

        soundManager.PlaySound("boxSwitchSE");
    }

    //残りターンを設定、更新
    public void SetRemainTurn()
    {
        remainTurnText.text = "TURN : " + remainTurn.ToString("D2");
    }

    //ステージ数を設定
    public void SetStageNum()
    {
        stageNameText.text = "Stage " + stageNum.ToString();
        gameManagerObject.GetComponent<GameManager>().GetStageNum(stageNum);
    }

    //StageClear時のUI表示
    public void SetActiveStageClearText()
    {
        stageClearUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
    }

    //GameOver時のUI表示
    public void SetActiveGameOverText()
    {
        gameOverUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
    }

    //戻るボタンを非表示に
    public void SetInactiveBackStageSelectButton()
    {
        backStageSelectButtonUI.SetActive(false);
    }

    //進むボタンを非表示に
    public void SetInactiveResetButton()
    {
        resetButtonUI.SetActive(false);
    }

    //UIを一括取得
    public void GetUIinGameScene()
    {
        stageClearUI = GameObject.Find("StageClearBase");
        gameOverUI = GameObject.Find("GameOverBase");
        backStageSelectButtonUI = GameObject.Find("BackStageSelectSceneBase");
        resetButtonUI = GameObject.Find("ResetBase");
        remainTurnText = GameObject.Find("RemainTurnText").GetComponent<TextMeshProUGUI>();
        stageNameText = GameObject.Find("StageNameText").GetComponent<TextMeshProUGUI>();
    }
}
