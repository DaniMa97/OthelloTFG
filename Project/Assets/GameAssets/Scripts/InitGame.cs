using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public GameObject smoothPanel;

    // Start is called before the first frame update
    void Start()
    {
        //Sound change
        StartCoroutine(ModifySound());
        smoothPanel.SetActive(true);
    }

    IEnumerator ModifySound()
    {
        yield return null;

        //Set Music 
        FadeMusic fm = FindObjectOfType<FadeMusic>();
        if(fm != null)
        {
            fm.FadeIn(Constants.FADE_TIME);
        }

        //Set SFX Sounds
        SoundController sc = FindObjectOfType<SoundController>();
        if (sc != null)
        {
            sc.SetSfxVolume(sc.GetSfxVolume());
        }
    }

}
