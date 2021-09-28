using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Map�̗l�X�ȏ��
public struct MapDatas
{
    public int stageNum;                                            //�X�e�[�W�ԍ�
    public int movableTime;                                         //�������
    public List<string[]> mapTile;                                  //�}�b�v�`�b�v�𔻕ʂ��邽�߂̋L��
    public int limitPosXmin, limitPosXmax;                          //X���̍ő�ƍŏ�
    public int limitPosYmin, limitPosYmax;                          //Y���̍ő�ƍŏ�
    public Dictionary<string, string> mapTileImageData;             //�}�b�v�`�b�v�̉摜�f�[�^
    public Dictionary<string, ObjectDatas> objectData;              //�}�b�v�`�b�v�ȊO�̃I�u�W�F�N�g�f�[�^
    public string immovableData;                                    //�����Ȃ��}�X�̃f�[�^
}

//�e�I�u�W�F�N�g���ێ�������
public struct ObjectDatas
{
    public string prefabName;
    public int posX;
    public int posY;
}

public class MapReader : MonoBehaviour
{
    private List<string[]> mapDataList = new List<string[]>();    //mapFile�̒��g��string�^�ɋN���������X�g
    private int lineNumber = 0;                                   //���݊m�F���̍s��
    private GameObject mapBaseObject;                             //�}�b�v�̐e�I�u�W�F�N�g
    private MapDatas mapDatas;                                    //�}�b�v�̃f�[�^�ꗗ

    public TextAsset mapFile;                                     //�}�b�v�f�[�^���L�^����.txt�t�@�C��
    public string stageName = "Stage1";                           //�X�e�[�W�̐e�A�X�e�[�W���ɂ��Ă���

    private void Awake()
    {
        //�}�b�v�f�[�^�̏�����
        InitializeMapData();
    }

    //�}�b�v�f�[�^�̏�����
    public void InitializeMapData()
    {
        GetMapData();      //.txt�t�@�C�����擾
        SetMapData();      //MapData�̌`�ɕϊ����ĕۑ�
    }

    //�ݒ肵��.txt�t�@�C����string��mapDataList�ɂ܂Ƃ߂ĕۑ�����
    private void GetMapData()
    {
        if(mapFile == null)
        {
            Debug.LogError("�}�b�v�f�[�^�����ݒ�ł��B");
            return;
        }

        StringReader reader = new StringReader(mapFile.text);

        while(reader.Peek() > -1)
        {
            string tmp = reader.ReadLine();
            mapDataList.Add(tmp.Split(','));
        }
    }

    //string�^�̗v�f�̏W���ł���mapDataList���A�e�f�[�^���Ƃɐ����y�ь^�ϊ����s��mapData�ɕۑ�����
    private void SetMapData()
    {
        mapBaseObject = GameObject.Find("StageParent").gameObject;

        lineNumber = 0;
        
        //��������mapDataList�̒��g��1�s�����Ă���mapData�ɂ��ꂼ��̏���ۑ�����
        while(lineNumber < mapDataList.Count)
        {
            //#START�`#END�܂ł���̃X�e�[�W�A1�s�����Ă���
            if (mapDataList[lineNumber][0] == "#START")
            {
                lineNumber++;

                //�e�X�e�[�W���ʂ̐e�I�u�W�F�N�g�̏�����
                mapDatas = new MapDatas
                {
                    objectData = new Dictionary<string, ObjectDatas>()
                };

                //Inspector�Őݒ肵���X�e�[�W���̃}�b�v�f�[�^��ǂݍ���
                if (stageName == mapDataList[lineNumber][0])
                {
                    mapDatas.stageNum = int.Parse(mapDataList[lineNumber][1]);
                    mapDatas.movableTime = int.Parse(mapDataList[lineNumber][2]);

                    //�e�p�[�g���Ƃɂ��ꂼ��̏���������
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

    //�}�b�v�̏��̏����L���Ŏ擾����
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
                Debug.LogError("MapTile���[�h���̃G���[�ł��B");
                return;
            }
        }
        mapDatas.mapTile = tmpMapTile;
    }

    //���ꂼ��̋L��������\���Ă��邩�̃f�[�^���擾����
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
                Debug.LogError("MapTileData���[�h���̃G���[�ł��B");
                return;
            }
        }

        mapDatas.immovableData = tmpImmovableData;
        mapDatas.mapTileImageData = tmpMapTileImageData;
    }

    //�}�b�v�ɔz�u����e�I�u�W�F�N�g(�v���C���[�┠)�̏����擾����
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
                Debug.LogError("ObjectData���[�h���̃G���[�ł��B");
                return;
            }

        }
    }
}
