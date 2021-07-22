using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsUI : MonoBehaviour
{
    public GameObject[] pages;
    public GameObject backButton, nextButton;

    byte actualPage = 0;

    // Start is called before the first frame update
    void Start()
    {
        ResetUI();
    }

    void OnDisable()
    {
        ResetUI();
    }

    void ResetUI()
    {
        pages[actualPage].SetActive(false);
        actualPage = 0;
        pages[actualPage].SetActive(true);
        backButton.SetActive(false);
        nextButton.SetActive(true);
    }

    public void ClickOnNext()
    {
        pages[actualPage].SetActive(false);
        actualPage++;
        pages[actualPage].SetActive(true);
        if (actualPage == 1)
        {
            backButton.SetActive(true);
        }
        else if(actualPage==Constants.NUM_PAGES_INSTRUCTIONS - 1)
        {
            nextButton.SetActive(false);
        }
    }

    public void ClickOnPrev()
    {
        pages[actualPage].SetActive(false);
        actualPage--;
        pages[actualPage].SetActive(true);
        if (actualPage == Constants.NUM_PAGES_INSTRUCTIONS - 2)
        {
            nextButton.SetActive(true);
        }
        else if (actualPage == 0)
        {
            backButton.SetActive(false);
        }
    }
}
