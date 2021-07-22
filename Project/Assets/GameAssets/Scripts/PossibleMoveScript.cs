using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleMoveScript : MonoBehaviour {

	public Material idle, turn1, turn2;

	void OnMouseDown(){
		GameController.instance.Click(this.transform.parent.parent.GetComponent<TileScript>(), true);
	}

	void OnMouseOver(){
		if (GameController.instance.isP1Turn) {
			this.GetComponent<Renderer> ().material = turn1;
		} else {
			this.GetComponent<Renderer> ().material = turn2;
		}
	}

	void OnMouseExit(){
		this.GetComponent<Renderer> ().material = idle;
	}
}
