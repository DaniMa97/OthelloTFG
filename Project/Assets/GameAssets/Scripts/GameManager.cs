using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool isPvIA, isIAP1;
    public int difficulty;
    public bool isOptionsOpen = false;

    bool smoothing = false;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //Smooth panel management
    public void LoadScene(string scene)
    {
        GameObject smoother = GameObject.FindGameObjectWithTag(Constants.TAG_SMOOTHER);
        if(smoother != null)
        {
            Animator anim = smoother.GetComponent<Animator>();
            anim.SetTrigger(Constants.SMOOTHER_TRIGGER);
            smoothing = true;

            //Fade out music 
            FadeMusic fm = FindObjectOfType<FadeMusic>();
            if (fm != null)
            {
                fm.FadeOut(Constants.FADE_TIME);
            }

            StartCoroutine(LoadSceneWithSmooth(scene));
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    IEnumerator LoadSceneWithSmooth(string scene)
    {
        while (smoothing)
        {
            yield return 0;
        }
        SceneManager.LoadScene(scene);
    }

    public void FinishSmooth()
    {
        smoothing = false;
    }
}