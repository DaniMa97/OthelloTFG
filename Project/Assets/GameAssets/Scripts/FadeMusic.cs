using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMusic : MonoBehaviour
{
    public void FadeIn(float fadeTime)
    {
        StartCoroutine(FadeInRoutine(fadeTime));
    }

    public void FadeOut(float fadeTime)
    {
        StartCoroutine(FadeOutRoutine(fadeTime));
    }

    IEnumerator FadeInRoutine(float fadeTime)
    {
        AudioSource asource = GetComponent<AudioSource>();
        if (asource != null)
        {
            float endVolume = GameManager.instance.GetComponent<SoundController>().GetMusicVolume();
            asource.volume = 0f;
            while (asource.volume < endVolume)
            {
                asource.volume += endVolume * Time.deltaTime / fadeTime;
                yield return null;
            }
        }
    }

    IEnumerator FadeOutRoutine(float fadeTime)
    {
        AudioSource asource = GetComponent<AudioSource>();
        if (asource != null)
        {
            float startVolume = asource.volume;
            while (asource.volume > 0)
            {
                asource.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }
            asource.Stop();
        }
    }
}
