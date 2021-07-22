using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public GameObject optionsPanel;
    public Button[] mainButtons;
    public GameObject[] confirmPanels;

    protected enum State { retry, mainMenu, exitGame};
    GameObject optionsButton;
    protected State s;

    //Options management
    protected virtual void Start()
    {
        foreach(GameObject go in confirmPanels)
        {
            go.SetActive(false);
        }
        optionsPanel.SetActive(false);
    }

    public void ClickOnOptions(GameObject button)
    {
        optionsButton = button;
        optionsPanel.SetActive(true);
        GameManager.instance.isOptionsOpen = true;
    }

    public void ClickOnBack()
    {
        if(optionsButton != null)
        {
            optionsButton.SetActive(true);
        }
        optionsPanel.SetActive(false);
        GameManager.instance.isOptionsOpen = false;
    }

    public void ClickOnRetry()
    {
        s = State.retry;
        ModifyConfirmPanels(true);
    }

    public void ClickOnMainMenu()
    {
        s = State.mainMenu;
        ModifyConfirmPanels(true);
    }

    public void ClickOnExit()
    {
        s = State.exitGame;
        ModifyConfirmPanels(true);
    }

    public void ClickOnConfirm()
    {
        foreach (Button b in mainButtons)
        {
            b.interactable = true;
        }
        foreach (GameObject go in confirmPanels)
        {
            go.SetActive(false);
        }
        optionsPanel.SetActive(false);
        GameManager.instance.isOptionsOpen = false;
        switch (s)
        {
            case State.retry:
                GameManager.instance.LoadScene(Constants.GAME_SCENE);
                break;
            case State.mainMenu:
                GameManager.instance.LoadScene(Constants.MAIN_MENU_SCENE);
                break;
            case State.exitGame:
                print("Quitting...");
                Application.Quit();
                break;
        }
    }

    public void ClickOnCancel()
    {
        ModifyConfirmPanels(false);
    }

    protected void ModifyConfirmPanels(bool activate)
    {
        foreach(Button b in mainButtons)
        {
            b.interactable = !activate;
        }
        switch (s)
        {
            case State.retry:
                confirmPanels[0].SetActive(activate);
                break;
            case State.mainMenu:
                confirmPanels[1].SetActive(activate);
                break;
            case State.exitGame:
                confirmPanels[2].SetActive(activate);
                break;
        }
    }
}
