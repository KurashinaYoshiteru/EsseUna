using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /*
    public GameObject stageClearUI;
    public static GameObject gameOverUI;
    public static GameObject backStageSelectButtonUI;
    public static GameObject resetButtonUI;
    public static Text stageNameText;
    public static Text remainTurnText;

    private GameObject stageClearUI;
    private GameObject gameOverUI;
    private GameObject backStageSelectButtonUI;
    private GameObject resetButtonUI;
    private Text stageNameText;
    private Text remainTurnText;
    */
    public GameObject fadeUIprefab;
    private GameObject fadeUI;

    public GameObject stageParent = null;

    //private int remainTurn = 99;
    public int stageNum;

    public enum GameState
    {
        TitleScene,
        StageSelectScene,
        GamePlay,
        GameOver,
        StageClear
    }
    public GameState currentState;

    public SoundManager soundManager;
    //public AudioSource audioSource;

    void Awake()
    {
        if(this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        currentState = GameState.TitleScene;

        playBGM("mainBGM");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playBGM(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.playSound(key);
    }
    public void playSE(string key)
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.playSound(key);
    }

    public async void LoadScene(string sceneName, float fadeInTime, float fadeOutTime, GameState state, bool chengeBGM)
    {
        fadeUI = Instantiate(fadeUIprefab);
        fadeUI.GetComponent<FadeManager>().StartFadeOut(fadeOutTime);
        await Task.Delay((int)(fadeOutTime * 1000.0f));
        SceneManager.LoadScene(sceneName);

        if(state == GameState.GamePlay)
        {
            if (chengeBGM)
            {
                playBGM("stage" + stageNum.ToString() + "BGM");
            }
            await Task.Delay((int)(fadeInTime * 500.0f));
        }
        else if(state == GameState.StageSelectScene)
        {
            if (chengeBGM)
            {
                playBGM("mainBGM");
            }
        }

        SetGameState(state);
        fadeUI.GetComponent<FadeManager>().StartFadeIn(fadeInTime);
        await Task.Delay((int)(fadeInTime * 1000.0f));
        //Destroy(fadeUI);
        DestroyImmediate(fadeUI);
    }
    

    public void SetGameState(GameState getState)
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().currentState = getState;
    }

    public void BackLoadStageSelectScene()
    {
        playSE("backSE");
        LoadScene("StageSelectScene", 1.0f, 1.0f, GameState.StageSelectScene, true);
        //playBGM("mainBGM");
    }

    public void ForwardLoadStageSelectScene()
    {
        playSE("decisionSE");
        LoadScene("StageSelectScene", 1.0f, 1.0f, GameState.StageSelectScene, false);
    }

    public void BackLoadTitleScene()
    {
        playSE("backSE");
        LoadScene("TitleScene", 1.0f, 1.0f, GameState.TitleScene, false);
    }

    public void ForwardLoadGameScene(int getStageNum)
    {
        playSE("decisionSE");
        stageNum = getStageNum;
        LoadScene("GameScene" + getStageNum.ToString(), 2.0f, 1.0f, GameState.GamePlay, true);
    }
    
    public void ResetLoadThisStageScene()
    {
        playSE("resetSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum;
        LoadScene("GameScene" + stageNum, 1.0f, 1.0f, GameState.GamePlay, false);
    }

    public void ForwardLoadNextStageScene()
    {
        playSE("decisionSE");
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum + 1;
        LoadScene("GameScene" + stageNum, 2.0f, 1.0f, GameState.GamePlay, true);
    }

    public void GetStageNum(int getValue)
    {
        stageNum = getValue;
    }

    /*
    IEnumerator LoadScene(string sceneName, float fadeInTime, float fadeOutTime, GameState state)
    {
        fadeUI = Instantiate(fadeUIprefab);
        fadeUI.GetComponent<FadeManager>().StartFadeOut(fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(sceneName);
        if(state == GameState.GamePlay)
        {
            //yield return StartCoroutine(GameObject.Find("StageParent").GetComponent<MapStatus>().CreateMapchips());
        }
        SetGameState(state);
        fadeUI.GetComponent<FadeManager>().StartFadeIn(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
        Destroy(fadeUI);
    }

    public void LoadThisStageScene()
    {
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum;
        LoadGameScene(stageNum);
    }

    public void LoadNextStageScene()
    {
        stageNum = GameObject.Find("StageParent").GetComponent<MapStatus>().stageNum;
        LoadGameScene(stageNum + 1);
        //GetUIinGameScene();
    }
    public void SetRemainTurn()
    {
        remainTurnText.text = "TURN : " + remainTurn.ToString("D2");
    }

    public void GetRemainTurn(int getValue)
    {
        remainTurn = getValue;
    }

    public void SetStageNum()
    {
        stageNameText.text = "Stage " + stageNum.ToString();
    }

    public void GetStageNum(int getValue)
    {
        stageNum = getValue;
    }

    public void SetActiveStageClearText()
    {
        stageClearUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
        currentState = GameState.StageClear;
    }

    public void SetActiveGameOverText()
    {
        gameOverUI.SetActive(true);
        SetInactiveBackStageSelectButton();
        SetInactiveResetButton();
        currentState = GameState.GameOver;
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
        Debug.Log("aa");
        stageClearUI = GameObject.Find("StageClearBase");
        gameOverUI = GameObject.Find("GameOverBase");
        backStageSelectButtonUI = GameObject.Find("BackStageSelectSceneBase");
        resetButtonUI = GameObject.Find("ResetBase");
        remainTurnText = GameObject.Find("RemainTurnText").GetComponent<Text>();
        stageNameText = GameObject.Find("StageNameText").GetComponent<Text>();
        Debug.Log("bb");
    }
    */
}
