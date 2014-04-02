using UnityEngine;
using System.Collections;

public class HudPlayerNumber : MonoBehaviour {
	
	private UILabel label;
	
	void Start() {
		label = GetComponent<UILabel>();
		label.text = "Player " + (NetworkManager.GetPlayerID()+1).ToString();
	}

}