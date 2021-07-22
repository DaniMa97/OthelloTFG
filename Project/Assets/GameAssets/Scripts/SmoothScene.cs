using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothScene : MonoBehaviour
{
    public void EndSmooth()
    {
        GameManager.instance.FinishSmooth();
    }
	
	public void EndSmoothMenu()
	{
        MenuMethods mm = FindObjectOfType<MenuMethods>();
        if (mm != null)
        {
            mm.FinishWaiting();
        }
	}
}
