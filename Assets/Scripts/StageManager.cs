using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    private static int unlockedStageNum;                     //����X�e�[�W��0�`7
    public GameObject[] stageLockUI = new GameObject[7];     //�X�e�[�W2�`�X�e�[�W8�܂ŁA�v�V�̊W�G

    private void Start()
    {
        //GameManager����X�e�[�W�N���A�����擾
        GameObject gameManagerObject = GameObject.Find("GameManager");
        unlockedStageNum = gameManagerObject.GetComponent<GameManager>().stageClearNum;

        //����X�e�[�W���ȉ��̊W�G���I�t�ɂ���
        for (int i = 0; i < unlockedStageNum; i++)
        {
            stageLockUI[i].SetActive(false);
        }
    }

}
