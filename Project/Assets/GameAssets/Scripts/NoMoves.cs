using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMoves : MonoBehaviour
{
    public UnityEngine.UI.Text text;

    void Start()
    {
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
        {
            text.text = "¡No hay movimientos disponibles!";
        }
    }

    void OnEnable()
    {
        GetComponent<AudioSource>().Play();
    }
}
