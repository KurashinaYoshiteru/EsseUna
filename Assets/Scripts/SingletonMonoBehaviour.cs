using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�V�[���ɕK����������݂��Ȃ��I�u�W�F�N�g�͂��̃N���X���p��������
//�{�v���W�F�N�g�ł�GameManager��SoundManager�̓��
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if(instance == null)
                {
                    Debug.LogError(typeof(T) + "���V�[���ɑ��݂��܂���");
                }
            }
            return instance;
        }
    }
}
