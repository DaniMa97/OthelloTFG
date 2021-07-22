using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThinkText : MonoBehaviour
{
    Text t;

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
        if (GameManager.instance.GetComponent<LanguageController>().GetIsSpanish())
            t.text = "Pensando...";
        else
            t.text = "Thinking...";
    }
}
