using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {

	public int indexX; 
	public int indexY;

	void OnMouseDown(){
		GameController.instance.Click (this, true);
	}
}
