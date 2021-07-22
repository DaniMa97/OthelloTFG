using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    public Text t1;
    public Text t2;
    public Text t3;
    public Text t4;
    public Image turnImage;
    public Sprite blackSprite, whiteSprite;

    LanguageController lan;

    void Start()
    {
        lan = GameManager.instance.GetComponent<LanguageController>();
        if (lan.GetIsSpanish())
        {
            t4.text = "Turno actual:";
        }
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        //Change points text
        if (lan.GetIsSpanish())
        {
            t1.text = "Puntos de las negras: " + GameController.instance.p1Pts;
            t2.text = "Puntos de las blancas: " + GameController.instance.p2Pts;
            if (GameController.instance.isP1Turn)
            {
                t3.text = "Negras";
                t3.color = Color.black;
            }
            else
            {
                t3.text = "Blancas";
                t3.color = Color.white;
            }
        }
        else
        {
            t1.text = "Black Player points: " + GameController.instance.p1Pts;
            t2.text = "White Player points: " + GameController.instance.p2Pts;
            if (GameController.instance.isP1Turn)
            {
                t3.text = "Black";
                t3.color = Color.black;
            }
            else
            {
                t3.text = "White";
                t3.color = Color.white;
            }
        }

        //Change UI Image
        if (GameController.instance.isP1Turn)
        {
            turnImage.sprite = blackSprite;
        }
        else
        {
            turnImage.sprite = whiteSprite;
        }
    }
}
