using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMethods : MonoBehaviour
{
    public GameObject mainPanel, playPanel, playersPanel, difficultyPanel, optionsPanel, smoothPanel, exitPanel;
    public GameObject howToPanelES, howToPanelEN;
    public Button creditsButton, languageButton;
    public GameObject imageLogo, textLogo, textInit;
    public Button[] mainButtons;
    public Slider musicSlider, sfxSlider;

    SoundController sc;

    bool waitingClick = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SearchSound());
		smoothPanel.SetActive(true);
        imageLogo.SetActive(true);
        textLogo.SetActive(true);
        textInit.SetActive(true);
        creditsButton.gameObject.SetActive(false);
        languageButton.gameObject.SetActive(false);
		mainPanel.SetActive(false);
        playPanel.SetActive(false);
        playersPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        howToPanelEN.SetActive(false);
        howToPanelES.SetActive(false);
        optionsPanel.SetActive(false);
        exitPanel.SetActive(false);
    }
	
	void Update(){
		if(waitingClick && Input.anyKeyDown){
            waitingClick = false;
			GetComponent<AudioSource>().Play();
            mainPanel.SetActive(true);
            languageButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            imageLogo.SetActive(false);
            textLogo.SetActive(false);
            textInit.SetActive(false);
        }
	}

    IEnumerator SearchSound()
    {
        yield return null;

        //Set Music 
        FadeMusic fm = FindObjectOfType<FadeMusic>();
        if (fm != null)
        {
            fm.FadeIn(Constants.FADE_TIME);
        }

        //Set sliders and sfx
        sc = FindObjectOfType<SoundController>();
        if (sc != null)
        {
            musicSlider.value = sc.GetMusicVolume();
            sfxSlider.value = sc.GetSfxVolume();
            sc.SetSfxVolume(sc.GetSfxVolume());
        }


    }

    public void FinishWaiting()
    {
        waitingClick = true;
    }

    public void ClickOnPvP()
    {
        GameManager.instance.isPvIA = false;
        GameManager.instance.LoadScene(Constants.GAME_SCENE);
    }

    public void ClickOnPvCPU()
    {
        playPanel.SetActive(false);
        playersPanel.SetActive(true);
        GameManager.instance.isPvIA = true;
    }

    public void ClickOnP1()
    {
        GameManager.instance.isIAP1 = false;
        playersPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void ClickOnP2()
    {
        GameManager.instance.isIAP1 = true;
        playersPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void ClickOnEasy()
    {
        GameManager.instance.difficulty = 0;
        GameManager.instance.LoadScene(Constants.GAME_SCENE);
    }

    public void ClickOnMedium()
    {
        GameManager.instance.difficulty = 1;
        GameManager.instance.LoadScene(Constants.GAME_SCENE);
    }

    public void ClickOnHard()
    {
        GameManager.instance.difficulty = 2;
        GameManager.instance.LoadScene(Constants.GAME_SCENE);
    }

    public void ClickOnBackPlayers()
    {
        playersPanel.SetActive(false);
        playPanel.SetActive(true);
    }

    public void ClickOnBackDifficulty()
    {
        playersPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void ClickOnBackGeneral(GameObject panel)
    {
        panel.SetActive(false);
        creditsButton.interactable = true;
        languageButton.interactable = true;
    }

    public void ActivatePanel(GameObject panel)
    {
        panel.SetActive(true);
        creditsButton.interactable = false;
    }

    public void ClickOnInstructions()
    {
        creditsButton.interactable = false;
        languageButton.interactable = false;
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            howToPanelES.SetActive(true);
        }
        else
        {
            howToPanelEN.SetActive(true);
        }
    }

    public void ClickOnExit(bool exit)
    {
        foreach (Button b in mainButtons)
            b.interactable = !exit;
        exitPanel.SetActive(exit);
    }

    public void ClickOnConfirm()
    {
        print("Quitting...");
        Application.Quit();
    }

    public void UpdateManagerMusicSlider(float v)
    {
        if (sc != null)
        {
            sc.musicSlider.value = v;
        }
    }

    public void UpdateManagerSFXSlider(float v)
    {
        if (sc != null)
        {
            sc.sfxSlider.value = v;
        }
    }
}
