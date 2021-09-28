using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{

    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public float alpha = 0.0f;
    public float fadeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            FadeIn();
        }
        if (isFadeOut)
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        alpha -= Time.deltaTime / fadeSpeed;
        if (alpha <= 0.0f)
        {
            isFadeIn = false;
            alpha = 0.0f;
        }
        this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
    }

    private void FadeOut()
    {
        alpha += Time.deltaTime / fadeSpeed;
        if (alpha >= 1.0f)
        {
            isFadeOut = false;
            alpha = 1.0f;
        }
        this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
    }

    public void StartFadeIn()
    {
        isFadeIn = true;
        isFadeOut = false;
    }

    public void StartFadeOut()
    {
        isFadeIn = false;
        isFadeOut = true;
    }
}
