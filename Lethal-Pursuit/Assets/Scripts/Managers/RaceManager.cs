using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class RaceManager : MonoBehaviour {


//	SpaceshipRaceData [] datas;
	HUD_Manager hudManager;


	// Use this for initialization
	void Start () {
//		datas = GameObject.FindObjectsOfType<SpaceshipRaceData>();
		hudManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUD_Manager>();
	}



	// Update is called once per frame
	void Update () {
//		spaceships.Sort(
//			(Spaceship lhs, Spaceship rhs) => lhs.CompareTo(rhs.transform.position.x)
//		);
	}



	public void OnFinishLap(SpaceshipRaceData raceData) {
		Debug.Log (raceData.gameObject.name + " lapped the race at " + raceData.finishLapTime + "!");
	}
	


	public void OnFinishRace(SpaceshipRaceData raceData) {
		Debug.Log (raceData.gameObject.name + " finished the race at " + raceData.finishRaceTime + "!");
		OnRaceOver();
	}


	public void OnRaceOver() {
		hudManager.DisplayRaceOver();
	}

}
