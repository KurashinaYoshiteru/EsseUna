using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private GameObject fadeUI;                     //�t�F�[�h�pUI�A�v���n�u�𐶐�����
    private GameObject tmpGameManager;             //�V�[���J�ڎ��Ɉꎞ�I�Ɏg����GameManager

    public GameObject fadeUIprefab;                //�t�F�[�hUI�v���n�u
    public SoundManager soundManager;              //�T�E���h�}�l�[�W���[
    public bool createMapchipsCompleted = false;   //�}�b�v�`�b�v�̐���������������true
    public int stageNum;                           //�X�e�[�W��
    public int stageClearNum = 0;                  //�X�e�[�W�N���A��

    //�Q�[���̏�ԗ񋓁A�V�[���J�ڂ̔��ʂɎg�p
    public enum GameState
    {
        TitleScene,
        StageSelectScene,
        GamePlay
    }


    void Awake()
    {
        //Singleton��
        if(this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        PlaySceneBGM("mainBGM");

    }

    //�w�肳�ꂽBGM�𗬂�
    public void PlaySceneBGM(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.PlaySound(key);
    }

    //�w�肳�ꂽSE�𗬂��AUIButton��LoadScene�͌Ăяo�����ߖ��xSoundManager��T���K�v������
    public void PlaySceneChangeSE(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.PlaySound(key);
    }

    //�V�[���J�ڂ͈�񂱂������ށA�e�V�[���J�ڊ֐���LosdScene_��LoadScene
    public void LoadScene_(string sceneName, GameState state, bool changeBGM)
    {
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene(sceneName, state, changeBGM));
    }

    //�t�F�[�h�̂��߂ɃR���[�`���Ŏg��
    IEnumerator LoadScene(string sceneName, GameState state, bool changeBGM)
    {
        fadeUI = Instantiate(fadeUIprefab);
        fadeUI.GetComponent<FadeManager>().StartFadeOut();
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);

        if (state == GameState.GamePlay)
        {
            if (changeBGM)
            {
                PlaySceneBGM("stage" + stageNum.ToString() + "BGM");
            }

            while (!tmpGameManager.GetComponent<GameManager>().createMapchipsCompleted)
            {
                yield return null;
            }
        }
        else if (state == GameState.StageSelectScene)
        {
            if (changeBGM)
            {
                PlaySceneBGM("mainBGM");
            }
        }

        fadeUI.GetComponent<FadeManager>().StartFadeIn();
        yield return new WaitForSeconds(1.0f);
        DestroyImmediate(fadeUI);
        tmpGameManager.GetComponent<GameManager>().createMapchipsCompleted = false;
    }

    //��������V�[���J�ڗpUIButton�ŌĂяo�����֐�

    //�^�C�g�����X�e�[�W�Z���N�g
    public void ForwardLoadStageSelectScene()
    {
        PlaySceneChangeSE("decisionSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("StageSelectScene", GameState.StageSelectScene, false));
    }

    //�^�C�g�����}�j���A��
    public void ForwardLoadManualScene()
    {
        PlaySceneChangeSE("decisionSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("ManualScene", GameState.StageSelectScene, false));
    }

    //�X�e�[�W�Z���N�g/�}�j���A�����^�C�g��
    public void BackLoadTitleScene()
    {
        PlaySceneChangeSE("backSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("TitleScene", GameState.TitleScene, false));
    }

    //�X�e�[�W�Z���N�g���X�e�[�W
    public void ForwardLoadGameScene(int getStageNum)
    {
        PlaySceneChangeSE("decisionSE");
        stageNum = getStageNum;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + getStageNum.ToString(), GameState.GamePlay, true));
    }

    //�X�e�[�W���X�e�[�W�Z���N�g
    public void BackLoadStageSelectScene()
    {
        PlaySceneChangeSE("backSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("StageSelectScene", GameState.StageSelectScene, true));
    }

    //�X�e�[�W���Z�b�g
    public void ResetLoadThisStageScene()
    {
        PlaySceneChangeSE("resetSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + stageNum, GameState.GamePlay, false));
    }

    //�l�N�X�g�X�e�[�W
    public void ForwardLoadNextStageScene()
    {
        PlaySceneChangeSE("decisionSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum + 1;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + stageNum, GameState.GamePlay, true));
    }

    //�X�e�[�W���擾�AMapStatus�ŌĂ΂��
    public void GetStageNum(int getValue)
    {
        stageNum = getValue;
    }
}
