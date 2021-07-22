using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    public void Activate(bool activate)
    {
        this.gameObject.SetActive(activate);
        GameManager.instance.GetComponent<OptionsController>().ClickOnOptions(this.gameObject);
    }
}
