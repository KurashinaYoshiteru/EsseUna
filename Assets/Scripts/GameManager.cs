using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private GameObject fadeUI;                     //フェード用UI、プレハブを生成する
    private GameObject tmpGameManager;             //シーン遷移時に一時的に使われるGameManager

    public GameObject fadeUIprefab;                //フェードUIプレハブ
    public bool createMapchipsCompleted = false;   //マップチップの生成が完了したらtrue
    public int stageNum;                           //ステージ数
    public SoundManager soundManager;              //サウンドマネージャー

    //ゲームの状態列挙、シーン遷移の判別に使用
    public enum GameState
    {
        TitleScene,
        StageSelectScene,
        GamePlay
    }


    void Awake()
    {
        //Singleton化
        if(this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        PlaySceneBGM("mainBGM");
    }

    //指定されたBGMを流す
    public void PlaySceneBGM(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.PlaySound(key);
    }

    //指定されたSEを流す、UIButtonでLoadSceneは呼び出すため毎度SoundManagerを探す必要がある
    public void PlaySceneChangeSE(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.PlaySound(key);
    }

    //
    public void LoadScene_(string sceneName, GameState state, bool changeBGM)
    {
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene(sceneName, state, changeBGM));
    }

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

    //ここからシーン遷移用UIButtonで呼び出される関数

    //タイトル→ステージセレクト
    public void ForwardLoadStageSelectScene()
    {
        PlaySceneChangeSE("decisionSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("StageSelectScene", GameState.StageSelectScene, false));
    }

    //ステージセレクト→タイトル
    public void BackLoadTitleScene()
    {
        PlaySceneChangeSE("backSE");
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("TitleScene", GameState.TitleScene, false));
    }

    //ステージセレクト→ステージ
    public void ForwardLoadGameScene(int getStageNum)
    {
        PlaySceneChangeSE("decisionSE");
        stageNum = getStageNum;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + getStageNum.ToString(), GameState.GamePlay, true));
    }

    //ステージ→ステージセレクト
    public void BackLoadStageSelectScene()
    {
        PlaySceneChangeSE("backSE");

        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("StageSelectScene", GameState.StageSelectScene, true));
    }

    //ステージリセット
    public void ResetLoadThisStageScene()
    {
        PlaySceneChangeSE("resetSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + stageNum, GameState.GamePlay, false));
    }

    //ネクストステージ
    public void ForwardLoadNextStageScene()
    {
        PlaySceneChangeSE("decisionSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum + 1;
        tmpGameManager = GameObject.Find("GameManager");
        tmpGameManager.GetComponent<GameManager>().StartCoroutine(LoadScene("GameScene" + stageNum, GameState.GamePlay, true));
    }

    //ステージ数取得、MapStatusで呼ばれる
    public void GetStageNum(int getValue)
    {
        stageNum = getValue;
    }
}
