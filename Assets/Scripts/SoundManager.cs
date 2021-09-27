using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private GameObject soundManager;
    //public AudioSource seAudioSource;
    //public AudioSource bgmAudioSource;
    private AudioSource seAudioSource;
    private AudioSource bgmAudioSource;

    public AudioClip mainBGM;
    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;
    public AudioClip stage4BGM;
    public AudioClip stage5BGM;
    public AudioClip stage6BGM;
    public AudioClip stage7BGM;
    public AudioClip stage8BGM;
    public AudioClip decisionSE;
    public AudioClip backSE;
    public AudioClip resetSE;
    public AudioClip stageClearSE;
    public AudioClip gameOverSE;
    public AudioClip mainPlayerMoveSE;
    public AudioClip subPlayerMoveSE;
    public AudioClip boxMoveSE;
    public AudioClip playerSwitchSE;
    public AudioClip boxSwitchSE;
    public AudioClip switchControllerSE;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        //soundManager = GameObject.Find("SoundManager");
        //audioSource = soundManager.GetComponent<AudioSource>();
        //audioSource.loop = true;
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void playSE(AudioClip clipSE)
    {
        seAudioSource.PlayOneShot(clipSE);
    }

    public void playBGM(AudioClip clipBGM)
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = clipBGM;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void playSound(string key)
    {
        switch (key)
        {
            case "mainBGM":
                playBGM(mainBGM);
                break;
            case "stage1BGM":
                playBGM(stage1BGM);
                break;
            case "stage2BGM":
                playBGM(stage2BGM);
                break;
            case "stage3BGM":
                playBGM(stage3BGM);
                break;
            case "stage4BGM":
                playBGM(stage4BGM);
                break;
            case "stage5BGM":
                playBGM(stage5BGM);
                break;
            case "stage6BGM":
                playBGM(stage6BGM);
                break;
            case "stage7BGM":
                playBGM(stage7BGM);
                break;
            case "stage8BGM":
                playBGM(stage8BGM);
                break;
            case "decisionSE":
                playSE(decisionSE);
                break;
            case "backSE":
                playSE(backSE);
                break;
            case "resetSE":
                playSE(resetSE);
                break;
            case "stageClearSE":
                playSE(stageClearSE);
                break;
            case "gameOverSE":
                playSE(gameOverSE);
                break;
            case "mainPlayerMoveSE":
                playSE(mainPlayerMoveSE);
                break;
            case "subPlayerMoveSE":
                playSE(subPlayerMoveSE);
                break;
            case "boxMoveSE":
                playSE(boxMoveSE);
                break;
            case "playerSwitchSE":
                playSE(playerSwitchSE);
                break;
            case "boxSwitchSE":
                playSE(boxSwitchSE);
                break;
            case "switchControllerSE":
                playSE(switchControllerSE);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
