using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRotation : MonoBehaviour {

	public bool needRotation, flipSound, isInit;
    public AudioClip flipClip;

	float total;
    AudioSource aSource;

	// Use this for initialization
	void Start () {
		needRotation = false;
		total = 180;
        aSource = GetComponent<AudioSource>();
        aSource.volume = GameManager.instance.GetComponent<SoundController>().GetSfxVolume();
        StartCoroutine(InitSound());
	}
	
    IEnumerator InitSound()
    {
        yield return null;
        if (!isInit)
        {
            aSource.Play();
        }
    }

	// Update is called once per frame
	void Update () {
		if (needRotation) {
            if (flipSound)
            {
                flipSound = false;
                aSource.PlayOneShot(flipClip, 1);
            }
			float variation = Constants.SPEED * Time.deltaTime;
			if (total - variation <= 0) {
				variation = total;
				total = 180;
				needRotation = false;
			} else {
				total -= variation;
			}
			this.transform.Rotate (variation, 0, 0);
		}
	}
}
