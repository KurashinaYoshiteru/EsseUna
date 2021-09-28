using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーンに必ず一つしか存在しないオブジェクトはこのクラスを継承させる
//本プロジェクトではGameManagerとSoundManagerの二つ
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
                    Debug.LogError(typeof(T) + "がシーンに存在しません");
                }
            }
            return instance;
        }
    }
}
