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
    //private GameObject boxParent;                //箱の親
    //private GameObject playerParent;             //プレイヤーの親
    private GameObject objectParent;             //オブジェクトの親

    //public GameObject gameManagerPrefab;
    private GameObject gameManager;
    private SoundManager soundManager;

    public GameObject mainPlayerPrefab;
    public GameObject subPlayerPrefab;
    public GameObject mainBoxPrefab;
    public GameObject subBoxPrefab;
    public GameObject boxPrefab;

    public int remainTurn;
    public int stageNum;

    public bool completeCreateMap = false;

    //マスの種類
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
        //PlayerOnNomal,    //
        //BoxOnNomal,       //
        //MainPlayer,       //プレイヤー1のいる位置
        //SubPlayer,        //プレイヤー2のいる位置
        //MainBox,          //プレイヤー1が押せる箱
        //SubBox,           //プレイヤー2が押せる箱
    }

    //方向
    public enum Direction
    {
        Left,
        Up,
        Right,
        Down,
    }


    public FloorKind[][] floorData;         //常に変わることのない床の種類のデータ
    public GameObject[][] objectData;    //常に更新されるマップ上のオブジェクト(プレイヤーや箱)などのデータ
    //public Dictionary<string, ObjectDatas> objectData = new Dictionary<string, ObjectDatas>();    //各オブジェクトが保持する情報

    private int goalPosX = 0;
    private int goalPosY = 0;

    private GameObject mainPlayer;
    private GameObject subPlayer;
    private List<GameObject> box = new List<GameObject>();

    public GameObject mainCamera;            //カメラ
    public float cameraBlank = 0;


    private GameObject stageClearUI;
    private GameObject gameOverUI;
    private GameObject backStageSelectButtonUI;
    private GameObject resetButtonUI;
    private TextMeshProUGUI stageNameText;
    private TextMeshProUGUI remainTurnText;




    /*
    public int playerPosX = 0;
    public int playerPosY = 0;
    public Dictionary<string, int> boxPosX = new Dictionary<string, int>();
    public Dictionary<string, int> boxPosY = new Dictionary<string, int>();
    private List<List<FloorKind>> currentMapStatus_;
    public bool startCreateMapTileImage = false;
    public bool startCreatePrefabs = false;
    */



    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //Debug.Log("マップ生成中...");
        StartCoroutine(CreateMapchips());

        //Debug.Log("オブジェクト生成中...");
        //StartCoroutine(CreateObjects_());
        CreateObjects();

        /*
        if (startCreateMapTileImage)
        {
            Debug.Log("マップ生成中...");

            if (mapData.immovableData == null)
            {
                Debug.LogError(this.name + "のマップが読み込めていない可能性があります。");
            }

            StartCoroutine(CreateMapTileImage());
            //startCreateMapTileImage = false;
        }

        if (startCreatePrefabs)
        {
            Debug.Log("オブジェクト生成中...");

            //EditorCoroutineUtility.StartCoroutine(CreatePrefabs(), this);
            StartCoroutine(CreatePrefabs());
            //startCreatePrefabs = false;
        }
        */


        //Invoke("InitializeFloorData", 0.1f);
        InitializeFloorData();
        //InitializeFloorData();    //マップの情報(floorDataとmapObjectData)を初期化
        SetCamera();            //カメラの位置セット

        //Debug.Log(CheckMovable(transform.gameObject, Direction.Left, 1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugMapData();
        }
    }

    //マップチップ生成
    public IEnumerator CreateMapchips()
    {
        //マップチップの親を指定
        if (GameObject.Find("MapTileImageParent"))
        {
            mapTileImageParent = GameObject.Find("MapTileImageParent");
        }

        /*
        else
        {
            mapTileImageParent = new GameObject("MapTileImageParent");
            mapTileImageParent.transform.parent = gameObject.transform;
            mapTileImageParent.transform.position = Vector3.zero;
        }
        for (int i = mapTileImageParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapTileImageParent.transform.GetChild(i).gameObject);
        }
        */


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
        //Debug.Log("マップ生成終了");

        completeCreateMap = true;
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

            objectData[tmpPosX][tmpPosY] = tmpGameObj;
        }
        //Debug.Log("オブジェクト生成終了");
    }


    //オブジェクト生成
    private IEnumerator CreateObjects_()
    {
        //mapObjectDataの初期化
        objectData = new GameObject[mapData.mapTile[0].Length][];
        for(int i = 0; i < mapData.mapTile[0].Length; i++)
        {
            objectData[i] = new GameObject[mapData.mapTile.Count];
            for(int j = 0; j < mapData.mapTile.Count; j++)
            {
                objectData[i][j] = null;
            }
        }

        /*
        //箱の親を指定
        if (GameObject.Find("BoxParent"))
        {
            boxParent = GameObject.Find("BoxParent");
        }
        else
        {
            boxParent = new GameObject("BoxParent");
            boxParent.transform.position = Vector3.zero;
        }
        for (int i = boxParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(boxParent.transform.GetChild(i).gameObject);
        }

        //プレイヤーの親を指定
        if (GameObject.Find("PlayerParent"))
        {
            playerParent = GameObject.Find("PlayerParent");
        }
        else
        {
            playerParent = new GameObject("PlayerParent");
            playerParent.transform.position = Vector3.zero;
        }
        for (int i = playerParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(playerParent.transform.GetChild(i).gameObject);
        }
        */

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
            string imageAddress = tmpData.prefabName;
            int tmpPosX = tmpData.posX;
            int tmpPosY = tmpData.posY;

            //GameObject tmpGameObj = null;
            //imageAddress = tmpData.prefabName;
            //tmpPosX = tmpData.posX;
            //tmpPosY = tmpData.posY;


            var tmpObj = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/" + imageAddress + ".prefab");
            yield return tmpObj;


            /*
            GameObject tmpObj = null;

            switch (imageAddress)
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
            GameObject tmpGameObj = Instantiate(tmpObj, objectParent.transform);
            */

            //オブジェクト生成等
            GameObject tmpGameObj = Instantiate(tmpObj.Result, objectParent.transform);
            tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
            tmpGameObj.transform.name = key;

            //生成したオブジェクトがプレイヤーか箱か判別
            if(tmpGameObj.tag == "MainPlayer" || tmpGameObj.tag == "SubPlayer")
            {
                //Player.csに情報を渡す
                tmpGameObj.GetComponent<Player>().mapStatus = this;
                tmpGameObj.GetComponent<Player>().playerPosX = tmpPosX;
                tmpGameObj.GetComponent<Player>().playerPosY = tmpPosY;
            }
            else if(tmpGameObj.tag == "MainBox" || tmpGameObj.tag == "SubBox" || tmpGameObj.tag == "Box")
            {
                //Box.csに情報を渡す
                tmpGameObj.GetComponent<Box>().mapStatus = this;
                tmpGameObj.GetComponent<Box>().boxPosX = tmpPosX;
                tmpGameObj.GetComponent<Box>().boxPosY = tmpPosY;
            }

            /*
            //生成するプレハブの名前によってそれぞれデータを渡す
            switch (imageAddress)
            {
                case "Cat":
                    tmpGameObj = Instantiate(tmpObj.Result, playerParent.transform);
                    tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
                    tmpGameObj.transform.name = key;

                    tmpGameObj.GetComponent<Player>().CurrentMapStatus = this;
                    tmpGameObj.GetComponent<Player>().playerPosX = tmpPosX;
                    tmpGameObj.GetComponent<Player>().playerPosY = tmpPosY;
                    //tmpGameObj.GetComponent<Player>().CurrentMapStatus.playerPosX = tmpPosX;
                    //tmpGameObj.GetComponent<Player>().CurrentMapStatus.playerPosY = tmpPosY;
                    break;
                case "BlackCat":
                    tmpGameObj = Instantiate(tmpObj.Result, playerParent.transform);
                    tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
                    tmpGameObj.transform.name = key;

                    tmpGameObj.GetComponent<Player>().CurrentMapStatus = this;
                    tmpGameObj.GetComponent<Player>().playerPosX = tmpPosX;
                    tmpGameObj.GetComponent<Player>().playerPosY = tmpPosY;
                    break;
                case "RedBox":
                    tmpGameObj = Instantiate(tmpObj.Result, boxParent.transform);
                    tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
                    tmpGameObj.transform.name = key;
                    tmpGameObj.GetComponent<Box>().CurrentMapStatus = this;
                    tmpGameObj.GetComponent<Box>().boxPosX = tmpPosX;
                    tmpGameObj.GetComponent<Box>().boxPosY = tmpPosY;
                    //tmpGameObj.GetComponent<Box>().CurrentMapStatus.boxPosX.Add(key, tmpPosX);
                    //tmpGameObj.GetComponent<Box>().CurrentMapStatus.boxPosY.Add(key, tmpPosY);
                    //tmpGameObj.GetComponent<Box>().boxPosX = tmpData.posX;
                    //tmpGameObj.GetComponent<Box>().boxPosY = tmpData.posY;
                    break;
                case "BlueBox":
                    tmpGameObj = Instantiate(tmpObj.Result, boxParent.transform);
                    tmpGameObj.transform.localPosition = new Vector2(tmpPosX, tmpPosY);
                    tmpGameObj.transform.name = key;
                    tmpGameObj.GetComponent<Box>().CurrentMapStatus = this;
                    tmpGameObj.GetComponent<Box>().boxPosX = tmpPosX;
                    tmpGameObj.GetComponent<Box>().boxPosY = tmpPosY;
                    break;
                default:
                    break;
            }

            ObjectDatas od = new ObjectDatas
            {
                prefabName = tmpData.prefabName,
                posX = tmpData.posX,
                posY = tmpData.posY
            };
            objectData.Add(key, od);
            */

            //objectData.Add(key, tmpData);

            objectData[tmpPosX][tmpPosY] = tmpGameObj;
        }
        //Debug.Log("オブジェクト生成終了");
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

                //Debug.Log(j + "-" + i + " = " + floorData[j][i]);
            }
        }

        /*
        gameManager.GetComponent<GameManager>().GetUIinGameScene();

        remainTurn = mapData.movableTime;
        gameManager.GetComponent<GameManager>().GetRemainTurn(remainTurn);
        gameManager.GetComponent<GameManager>().SetRemainTurn();
        stageNum = mapData.stageNum;
        gameManager.GetComponent<GameManager>().GetStageNum(stageNum);
        gameManager.GetComponent<GameManager>().SetStageNum();
        */

        remainTurn = mapData.movableTime;
        stageNum = mapData.stageNum;
        GetUIinGameScene();

        stageClearUI.SetActive(false);
        gameOverUI.SetActive(false);

        SetRemainTurn();
        SetStageNum();

    }

    //objectDataを更新する、毎回ゴールチェックもする
    public void UpdateMapData(int posX, int posY, GameObject gameObject)
    {
        //floorData[posX][posY] = floorKind;
        objectData[posX][posY] = gameObject;
        //CheckGoal();
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
                        soundManager.playSound("mainPlayerMoveSE");
                    }
                    //プレイヤーの場合、押せる箱であるか見る
                    else if (objectData[checkPosX][checkPosY].tag == "MainBox" || objectData[checkPosX][checkPosY].tag == "Box")
                    {
                        //押せる箱ならその箱は位置的に押せる箱かを見る
                        if (CheckMovable(objectData[checkPosX][checkPosY], dir, checkPosX, checkPosY))
                        {
                            returnValue = true;
                            soundManager.playSound("mainPlayerMoveSE");
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
                        soundManager.playSound("subPlayerMoveSE");
                    }
                    else if (objectData[checkPosX][checkPosY].tag == "SubBox" || objectData[checkPosX][checkPosY].tag == "Box")
                    {
                        if (CheckMovable(objectData[checkPosX][checkPosY], dir, checkPosX, checkPosY))
                        {
                            returnValue = true;
                            soundManager.playSound("subPlayerMoveSE");
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
                        soundManager.playSound("boxMoveSE");
                    }
                }
                break;
            case "MainBox":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.SubPlayerOnly)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.playSound("boxMoveSE");
                    }
                }
                break;
            case "SubBox":
                if (returnFloor == FloorKind.Nomal || returnFloor == FloorKind.MainPlayerOnly || returnFloor == FloorKind.SubPlayerOnly)
                {
                    if (objectData[checkPosX][checkPosY] == null)
                    {
                        returnValue = true;
                        soundManager.playSound("boxMoveSE");
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
        if (centerX * 9.0f / 16.0f > centerY)
        {
            mainCamera.GetComponent<Camera>().orthographicSize = (centerX + blank) * 9.0f / 16.0f;
        }
        else
        {
            mainCamera.GetComponent<Camera>().orthographicSize = centerY + blank;
        }
    }

    //デバッグ用
    void DebugMapData()
    {
        /*
        for (int i = 0; i < floorData.Length; i++)
        {
            for (int j = 0; j < floorData[0].Length; j++)
            {
                Debug.Log(i + "-" + j + "=" + floorData[i][j]);
            }
        }
        */

        //Debug.Log("MainPlayer posX : " + mainPlayer.GetComponent<Player>().playerPosX);

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
        //gameManager.GetComponent<GameManager>().GetRemainTurn(remainTurn);
        //gameManager.GetComponent<GameManager>().SetRemainTurn();
        SetRemainTurn();
        if (remainTurn <= 0)
        {
            //Debug.Log("GAME OVER...");
            GameOver();
        }

    }

    //ゲームオーバー時の処理
    void GameOver()
    {
        RemovePlayerComponent();
        //gameManager.GetComponent<GameManager>().SetActiveGameOverText();
        SetActiveGameOverText();
        soundManager.playSound("gameOverSE");
    }

    //ゴール時の処理
    void Goal()
    {
        RemovePlayerComponent();
        //gameManager.GetComponent<GameManager>().SetActiveStageClearText();
        SetActiveStageClearText();
        soundManager.playSound("stageClearSE");
    }

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
                //OnClearBoxSwitch();
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

        soundManager.playSound("playerSwitchSE");
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

        soundManager.playSound("boxSwitchSE");
    }


    public void SetRemainTurn()
    {
        remainTurnText.text = "TURN : " + remainTurn.ToString("D2");
    }


    public void SetStageNum()
    {
        stageNameText.text = "Stage " + stageNum.ToString();
        gameManager.GetComponent<GameManager>().GetStageNum(stageNum);
    }


    public void SetActiveStageClearText()
    {
        stageClearUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
        //currentState = GameState.StageClear;
    }

    public void SetActiveGameOverText()
    {
        gameOverUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
        //currentState = GameState.GameOver;
    }

    public void SetInactiveBackStageSelectButton()
    {
        backStageSelectButtonUI.SetActive(false);
    }

    public void SetInactiveResetButton()
    {
        resetButtonUI.SetActive(false);
    }

    public void GetUIinGameScene()
    {
        stageClearUI = GameObject.Find("StageClearBase");
        gameOverUI = GameObject.Find("GameOverBase");
        backStageSelectButtonUI = GameObject.Find("BackStageSelectSceneBase");
        resetButtonUI = GameObject.Find("ResetBase");
        remainTurnText = GameObject.Find("RemainTurnText").GetComponent<TextMeshProUGUI>();
        stageNameText = GameObject.Find("StageNameText").GetComponent<TextMeshProUGUI>();
    }


    /*

    //dirの方向に動けるか確認する、
    public bool CheckMovable_(Direction dir, int posX, int posY)
    {
        bool returnValue = true;

        switch (dir)
        {
            case Direction.Left:
                if (IsImmovableFloor(posX - 1, posY)) returnValue = false;
                break;
            case Direction.Up:
                if (IsImmovableFloor(posX, posY + 1)) returnValue = false;
                break;
            case Direction.Right:
                if (IsImmovableFloor(posX + 1, posY)) returnValue = false;
                break;
            case Direction.Down:
                if (IsImmovableFloor(posX, posY - 1)) returnValue = false;
                break;
        }

        return returnValue;
    }

    private bool IsImmovableFloor(int i, int j)
    {
        if ((i < mapData.limitPosXmin) || (i > mapData.limitPosXmax)
        || (j < mapData.limitPosYmin) || (j > mapData.limitPosYmax))
        {
            return true;
        }

        if (floorData[i][j] == FloorKind.Immovable || floorData[i][j] == FloorKind.PlayerOnNomal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    */

    /*
    public void SetCurrentMap(int firstPlayerPosX, int firstPlayerPosY)
    {
        Debug.LogError("SetCurrentMap");
        GameObject.Find("Player").GetComponent<Player>().CurrentMapStatus = this;

        playerPosX = firstPlayerPosX;
        playerPosY = firstPlayerPosY;



        GameObject.Find("Player").transform.position = new Vector2(mapData.limitPosXmin + playerPosX, mapData.limitPosYmin + playerPosY);
    }

    public bool CheckMovable_(Direction inputDir)
    {
        bool returnValue = true;

        switch (inputDir)
        {
            case Direction.Left:
                if (IsImmovableFloor(playerPosX - 1, playerPosY)) returnValue = false;
                break;
            case Direction.Up:
                if (IsImmovableFloor(playerPosX, playerPosY + 1)) returnValue = false;
                break;
            case Direction.Right:
                if (IsImmovableFloor(playerPosX + 1, playerPosY)) returnValue = false;
                break;
            case Direction.Down:
                if (IsImmovableFloor(playerPosX, playerPosY - 1)) returnValue = false;
                break;
        }

        return returnValue;
    }

    public bool CheckMovable__(Direction dir)
    {
        bool movable = true;
        int difX = 0;
        int difY = 0;

        switch (dir)
        {
            case Direction.Left:
                difX = -1;
                break;
            case Direction.Up:
                difY = -1;
                break;
            case Direction.Right:
                difX = 1;
                break;
            case Direction.Down:
                difX = 1;
                break;
            default:
                return false;
        }

        foreach(string key in mapData.changeMapData.Keys)
        {
            if(mapData.mapTile[playerPosY + difY][playerPosX + difX] == key)
            {
                ChangeMapDatas cmDatas = new ChangeMapDatas();
                mapData.changeMapData.TryGetValue(key, out cmDatas);

                if(GameObject.Find(cmDatas.mapName) == null)
                {
                    Debug.LogError(cmDatas.mapName + "というマップは存在しません。");
                    return false;
                }

                GameObject.Find(cmDatas.mapName).GetComponent<MapStatus>().SetCurrentMap(cmDatas.playerPosX, playerPosY);
                return false;
            }
        }

        for(int i = 0; i < mapData.immovableData.Length; i++)
        {
            if(mapData.mapTile[playerPosY + difY][playerPosX + difX] == mapData.immovableData[i].ToString())
            {
                movable = false;
            }
        }

        if (movable)
        {
            playerPosX += difX;
            playerPosY += difY;
        } 
        
        return movable;
    }



    */

}
