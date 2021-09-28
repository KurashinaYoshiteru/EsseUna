using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private AudioSource bgmAudioSource;          //BGM�p�̃I�[�f�B�I�\�[�X
    private AudioSource seAudioSource;           //SE�p�̃I�[�f�B�I�\�[�X

    //�C���X�y�N�^�[�Ŋe�������Z�b�g
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
        //Singleton��
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        //2�̃I�[�f�B�I�\�[�X��������
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();
    }

    //BGM�Đ�
    private void PlayBGM(AudioClip clipBGM)
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = clipBGM;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    //SE�Đ�
    private void PlaySE(AudioClip clipSE)
    {
        seAudioSource.PlayOneShot(clipSE);
    }

    //BGM��SE�ꊇ�ł܂Ƃ߂�A�T�E���h�͑S�Ă��̊֐����ĂԂ��Ƃɂ���
    public void PlaySound(string key)
    {
        switch (key)
        {
            case "mainBGM":
                PlayBGM(mainBGM);
                break;
            case "stage1BGM":
                PlayBGM(stage1BGM);
                break;
            case "stage2BGM":
                PlayBGM(stage2BGM);
                break;
            case "stage3BGM":
                PlayBGM(stage3BGM);
                break;
            case "stage4BGM":
                PlayBGM(stage4BGM);
                break;
            case "stage5BGM":
                PlayBGM(stage5BGM);
                break;
            case "stage6BGM":
                PlayBGM(stage6BGM);
                break;
            case "stage7BGM":
                PlayBGM(stage7BGM);
                break;
            case "stage8BGM":
                PlayBGM(stage8BGM);
                break;
            case "decisionSE":
                PlaySE(decisionSE);
                break;
            case "backSE":
                PlaySE(backSE);
                break;
            case "resetSE":
                PlaySE(resetSE);
                break;
            case "stageClearSE":
                PlaySE(stageClearSE);
                break;
            case "gameOverSE":
                PlaySE(gameOverSE);
                break;
            case "mainPlayerMoveSE":
                PlaySE(mainPlayerMoveSE);
                break;
            case "subPlayerMoveSE":
                PlaySE(subPlayerMoveSE);
                break;
            case "boxMoveSE":
                PlaySE(boxMoveSE);
                break;
            case "playerSwitchSE":
                PlaySE(playerSwitchSE);
                break;
            case "boxSwitchSE":
                PlaySE(boxSwitchSE);
                break;
            case "switchControllerSE":
                PlaySE(switchControllerSE);
                break;
        }
    }

}
