using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sound management
public class SoundController : MonoBehaviour
{
    public UnityEngine.UI.Slider musicSlider, sfxSlider;

    float musicVolume = 1, sfxVolume = 1;

    public float GetMusicVolume() { return musicVolume; }
    public float GetSfxVolume() { return sfxVolume; }

    public void SetMusicVolume(float v)
    {
        musicVolume = v;
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audios)
        {
            if (source.loop)
                source.volume = v;
        }
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = v;
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audios)
        {
            if (!source.loop)
                source.volume = v;
        }
    }
}
