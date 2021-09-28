using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Mapの様々な情報
public struct MapDatas
{
    public int stageNum;                                            //ステージ番号
    public int movableTime;                                         //動ける回数
    public List<string[]> mapTile;                                  //マップチップを判別するための記号
    public int limitPosXmin, limitPosXmax;                          //X軸の最大と最小
    public int limitPosYmin, limitPosYmax;                          //Y軸の最大と最小
    public Dictionary<string, string> mapTileImageData;             //マップチップの画像データ
    public Dictionary<string, ObjectDatas> objectData;              //マップチップ以外のオブジェクトデータ
    public string immovableData;                                    //動けないマスのデータ
}

//各オブジェクトが保持する情報
public struct ObjectDatas
{
    public string prefabName;
    public int posX;
    public int posY;
}

public class MapReader : MonoBehaviour
{
    private List<string[]> mapDataList = new List<string[]>();    //mapFileの中身をstring型に起こしたリスト
    private int lineNumber = 0;                                   //現在確認中の行数
    private GameObject mapBaseObject;                             //マップの親オブジェクト
    private MapDatas mapDatas;                                    //マップのデータ一覧

    public TextAsset mapFile;                                     //マップデータを記録した.txtファイル
    public string stageName = "Stage1";                           //ステージの親、ステージ名にしておく

    private void Awake()
    {
        //マップデータの初期化
        InitializeMapData();
    }

    //マップデータの初期化
    public void InitializeMapData()
    {
        GetMapData();      //.txtファイルを取得
        SetMapData();      //MapDataの形に変換して保存
    }

    //設定した.txtファイルをstringでmapDataListにまとめて保存する
    private void GetMapData()
    {
        if(mapFile == null)
        {
            Debug.LogError("マップデータが未設定です。");
            return;
        }

        StringReader reader = new StringReader(mapFile.text);

        while(reader.Peek() > -1)
        {
            string tmp = reader.ReadLine();
            mapDataList.Add(tmp.Split(','));
        }
    }

    //string型の要素の集合であるmapDataListを、各データごとに整理及び型変換を行いmapDataに保存する
    private void SetMapData()
    {
        mapBaseObject = GameObject.Find("StageParent").gameObject;

        lineNumber = 0;
        
        //ここからmapDataListの中身を1行ずつ見ていきmapDataにそれぞれの情報を保存する
        while(lineNumber < mapDataList.Count)
        {
            //#START～#ENDまでが一つのステージ、1行ずつ見ていく
            if (mapDataList[lineNumber][0] == "#START")
            {
                lineNumber++;

                //各ステージ共通の親オブジェクトの初期化
                mapDatas = new MapDatas
                {
                    objectData = new Dictionary<string, ObjectDatas>()
                };

                //Inspectorで設定したステージ名のマップデータを読み込む
                if (stageName == mapDataList[lineNumber][0])
                {
                    mapDatas.stageNum = int.Parse(mapDataList[lineNumber][1]);
                    mapDatas.movableTime = int.Parse(mapDataList[lineNumber][2]);

                    //各パートごとにそれぞれの処理をする
                    while (mapDataList[lineNumber][0] != "#END" && lineNumber < mapDataList.Count)
                    {
                        lineNumber++;

                        switch (mapDataList[lineNumber][0])
                        {
                            case "MapTileStart":
                                LoadMapTile();
                                break;
                            case "MapTileDataStart":
                                LoadMapTileData();
                                break;
                            case "ObjectDataStart":
                                LoadObjectData();
                                break;
                            default:
                                break;
                        }
                    }
                    mapBaseObject.GetComponent<MapStatus>().SetMapData(mapDatas);
                }
            }
            lineNumber++;
        }
    }

    //マップの床の情報を記号で取得する
    private void LoadMapTile()
    {
        List<string[]> tmpMapTile = new List<string[]>();
        lineNumber++;

        while (mapDataList[lineNumber][0] != "MapTileEnd")
        {
            tmpMapTile.Add(mapDataList[lineNumber]);

            lineNumber++;

            if (lineNumber >= mapDataList.Count)
            {
                Debug.LogError("MapTileロード時のエラーです。");
                return;
            }
        }
        mapDatas.mapTile = tmpMapTile;
    }

    //それぞれの記号が何を表しているかのデータを取得する
    private void LoadMapTileData()
    {
        string tmpImmovableData = "";
        Dictionary<string, string> tmpMapTileImageData = new Dictionary<string, string>();
        lineNumber++;

        while(mapDataList[lineNumber][0] != "MapTileDataEnd")
        {
            if(mapDataList[lineNumber][0] == "Immovable")
            {
                for(int i = 1; i < mapDataList[lineNumber].Length; i++)
                {
                    tmpImmovableData += mapDataList[lineNumber][i];
                }
            }
            else
            {
                tmpMapTileImageData.Add(mapDataList[lineNumber][0], mapDataList[lineNumber][1]);
            }

            lineNumber++;

            if (lineNumber >= mapDataList.Count)
            {
                Debug.LogError("MapTileDataロード時のエラーです。");
                return;
            }
        }

        mapDatas.immovableData = tmpImmovableData;
        mapDatas.mapTileImageData = tmpMapTileImageData;
    }

    //マップに配置する各オブジェクト(プレイヤーや箱)の情報を取得する
    private void LoadObjectData()
    {
        lineNumber++;

        while (mapDataList[lineNumber][0] != "ObjectDataEnd")
        {
            ObjectDatas moDatas = new ObjectDatas
            {
                prefabName = mapDataList[lineNumber][1],
                posX = int.Parse(mapDataList[lineNumber][2]),
                posY = int.Parse(mapDataList[lineNumber][3]),
            };

            mapDatas.objectData.Add(mapDataList[lineNumber][0], moDatas);
            lineNumber++;

            if (lineNumber >= mapDataList.Count)
            {
                Debug.LogError("ObjectDataロード時のエラーです。");
                return;
            }

        }
    }
}
