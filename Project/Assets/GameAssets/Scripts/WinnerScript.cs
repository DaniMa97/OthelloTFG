using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerScript : OptionsController
{
    public GameObject buttonGameOver;
    public Text winnerText;
    public Button optsButton;

    //Main panel
    public Text titleGameOver;

    //Buttons
    public Text board;
    public Text retry;
    public Text menu;
    public Text exitGame;

    //Retry panel
    public Text retryText;
    public Text confirmRetry;

    //Main menu panel
    public Text menuText;
    public Text confirmMenu;

    //Exit game panel
    public Text exitTextManager;
    public Text confirmExitManager;


    public void AppearDissapearGameOver(bool v)
    {
        optsButton.interactable = !v;
        buttonGameOver.SetActive(!v);
        optionsPanel.SetActive(v);
    }

    public void Winner(int pWinner)
    {
        optionsPanel.SetActive(true);
        optsButton.interactable = false;
        GetComponent<AudioSource>().Play();
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audios)
        {
            if (source.loop)
                source.Stop();
        }
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            switch (pWinner)
            {
                case 1:
                    winnerText.text = "¡Las negras ganan la partida!";
                    break;
                case 2:
                    winnerText.text = "¡Las blancas ganan la partida!";
                    break;
                case 0:
                    winnerText.text = "¡La partida termina en empate!";
                    break;
                default:
                    winnerText.text = "Error en la resolucion de juego";
                    break;
            }
        }
        else
        {
            switch (pWinner)
            {
                case 1:
                    winnerText.text = "Black player wins the match!";
                    break;
                case 2:
                    winnerText.text = "White player wins the match!";
                    break;
                case 0:
                    winnerText.text = "The match ended in a draw!";
                    break;
                default:
                    winnerText.text = "Error at the end of the game.";
                    break;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        buttonGameOver.SetActive(false);
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
            SpanishText();
    }

    void SpanishText()
    {
        titleGameOver.text = "Fin del juego";

        board.text = "Ver tablero";
        retry.text = "Volver a empezar";
        menu.text = "Menú principal";
        exitGame.text = "Salir \ndel juego";

        retryText.text = "¿Seguro que quieres volver a empezar el juego?";
        confirmRetry.text = "Sí";

        menuText.text = "¿Quieres volver al menú principal?";
        confirmMenu.text = "Sí";

        exitTextManager.text = "¿Seguro que quieres salir del juego?";
        confirmExitManager.text = "Sí";
    }
}