using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    bool isSpanish = false;

    public bool GetIsSpanish() { return isSpanish; }
    public void SetIsSpanish(bool v)
    {
        ChangeMode(v);
        isSpanish = v;
    }

    //Panel game controller
    public Text titleOptionsIngame;
    public Text musicIngame;
    public Text sfxIngame;

    //Buttons game controller
    public Text back;
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

    public void ChangeMode(bool v)
    {
        if (isSpanish)
        {
            titleOptionsIngame.text = "Options";
            musicIngame.text = "Music volume";
            sfxIngame.text = "SFX volume";

            back.text = "Back";
            retry.text = "Retry";
            menu.text = "Main Menu";
            exitGame.text = "Exit Game";

            retryText.text = "Are you sure you want to restart the game? \n(All progress on the match will be lost)";
            confirmRetry.text = "Yes";

            menuText.text = "Are you sure you want to go return to the main menu? \n(All progress on the match will be lost)";
            confirmMenu.text = "Yes";

            exitTextManager.text = "Are you sure you want to exit the game? \n(All progress on the match will be lost)";
            confirmExitManager.text = "Yes";
        }

        else
        {
            titleOptionsIngame.text = "Opciones";
            musicIngame.text = "Volumen de la música";
            sfxIngame.text = "Volumen de los efectos de sonido";

            back.text = "Atrás";
            retry.text = "Volver a empezar";
            menu.text = "Menú principal";
            exitGame.text = "Salir \ndel juego";

            retryText.text = "¿Seguro que quieres volver a empezar el juego? \n(Todo el progreso de la partida se perderá)";
            confirmRetry.text = "Sí";

            menuText.text = "¿Seguro que quieres volver al menú principal? \n(Todo el progreso de la partida se perderá)";
            confirmMenu.text = "Sí";

            exitTextManager.text = "¿Seguro que quieres salir del juego? \n(Todo el progreso de la partida se perderá)";
            confirmExitManager.text = "Sí";
        }
    }
}
