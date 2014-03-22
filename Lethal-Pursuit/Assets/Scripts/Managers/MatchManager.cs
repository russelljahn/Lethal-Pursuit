using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class MatchManager : MonoBehaviour {

	
	HudManager hudManager;


	// Use this for initialization
	void Start () {
		hudManager = GameObject.FindGameObjectWithTag("HudManager").GetComponent<HudManager>();
	}



	// Update is called once per frame
	void Update () {
//		spaceships.Sort(
//			(Spaceship lhs, Spaceship rhs) => lhs.CompareTo(rhs.transform.position.x)
//		);
	}


	public void OnMatchOver(SpaceshipMatchData matchData) {
		Debug.Log ("Match is over for '"  + matchData.spaceship + "'!");
		OnMatchOverGUI();
	}


	public void OnMatchOverGUI() {
		hudManager.DisplayMatchOver();
	}

}
