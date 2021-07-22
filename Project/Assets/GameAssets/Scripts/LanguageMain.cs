using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageMain : MonoBehaviour
{
    public Text langText;
    public Text creditsText;
    public Text initText;
    public Image flag;
    public Sprite esFlag;
    public Sprite ukFlag;

    [System.Serializable]
    public class TextObjects{
        //Main panel
        public Text playText;
        public Text howToText;
        public Text optionsText;
        public Text exitText;

        //Panel credits
        public Text titleCredits;
        public Text backCredits;

        //Panel play
        public Text titlePlay;
        public Text pvp;
        public Text pvia;
        public Text exitPlay;

        //Panel players
        public Text titlePlayers;
        public Text black;
        public Text white;
        public Text exitPlayers;

        //Panel dificulty
        public Text titleDificulty;
        public Text easy;
        public Text medium;
        public Text hard;
        public Text exitDifficulty;

        //Panel options
        public Text titleOptions;
        public Text music;
        public Text sfx;
        public Text exitOptions;

        //Panel confirm exit
        public Text exitPanelText;
        public Text confirmExit;
    }

    public TextObjects textObjects;

    public void ClickOnLanguage()
    {
        GameManager.instance.gameObject.GetComponent<LanguageController>().SetIsSpanish(!GameManager.instance.GetComponent<LanguageController>().GetIsSpanish());
        ChangeButton();
        ChangeText();
    }

    void ChangeButton()
    {
        if (!GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            langText.text = "EN";
            creditsText.text = "Credits";
            flag.sprite = ukFlag;
        }
        else
        {
            langText.text = "ES";
            creditsText.text = "Créditos";
            flag.sprite = esFlag;
        }
    }
    void ChangeText()
    {
        if (!GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            textObjects.titleCredits.text = "Credits";
            textObjects.backCredits.text = "Back";

            textObjects.playText.text = "Play";
            textObjects.howToText.text = "How to play";
            textObjects.optionsText.text = "Options";
            textObjects.exitText.text = "Exit game";

            textObjects.titlePlay.text = "Select Mode";
            textObjects.pvp.text = "Player vs Player";
            textObjects.pvia.text = "Player vs CPU";
            textObjects.exitPlay.text = "Back";

            textObjects.titlePlayers.text = "Select Player";
            textObjects.black.text = "Black";
            textObjects.white.text = "White";
            textObjects.exitPlayers.text = "Back";

            textObjects.titleDificulty.text = "Select Difficulty";
            textObjects.easy.text = "Easy";
            textObjects.medium.text = "Medium";
            textObjects.hard.text = "Hard";
            textObjects.exitDifficulty.text = "Back";

            textObjects.exitPanelText.text = "Are you sure you want to exit the game?";
            textObjects.confirmExit.text = "Yes";

            textObjects.titleOptions.text = "Options";
            textObjects.music.text = "Music volume";
            textObjects.sfx.text = "SFX volume";
            textObjects.exitOptions.text = "Back";
        }
        else
        {
            textObjects.titleCredits.text = "Créditos";
            textObjects.backCredits.text = "Atrás";

            textObjects.playText.text = "Jugar";
            textObjects.howToText.text = "Cómo jugar";
            textObjects.optionsText.text = "Opciones";
            textObjects.exitText.text = "Salir del juego";

            textObjects.titlePlay.text = "Elige modo de juego";
            textObjects.pvp.text = "Jugador vs Jugador";
            textObjects.pvia.text = "Jugador vs CPU";
            textObjects.exitPlay.text = "Atrás";

            textObjects.titlePlayers.text = "Elige jugador";
            textObjects.black.text = "Negras";
            textObjects.white.text = "Blancas";
            textObjects.exitPlayers.text = "Atrás";

            textObjects.titleDificulty.text = "Elige dificultad";
            textObjects.easy.text = "Fácil";
            textObjects.medium.text = "Normal";
            textObjects.hard.text = "Difícil";
            textObjects.exitDifficulty.text = "Atrás";

            textObjects.titleOptions.text = "Opciones";
            textObjects.music.text = "Volumen de la música";
            textObjects.sfx.text = "Volumen de los efectos de sonido";
            textObjects.exitOptions.text = "Atrás";

            textObjects.exitPanelText.text = "¿Seguro que quieres salir del juego?";
            textObjects.confirmExit.text = "Sí";
        }
    }

    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return null;
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            ChangeButton();
            ChangeText();
            initText.text = "Pulsa cualquier tecla para empezar";
        }
    }
}
