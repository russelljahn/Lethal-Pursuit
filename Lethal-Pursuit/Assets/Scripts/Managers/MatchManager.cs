using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public enum MatchMode {
	BATTLE,
//	TEAM_BATTLE,
//	CAPTURE_THE_FLAG,
//	JUGGERNAUT
}


public enum MatchRule {
	REACH_TARGET_KILLS,
//	MAX_KILLS_IN_TIME_LIMIT,
}


public class MatchManager : MonoBehaviour {

	public MatchMode mode = MatchMode.BATTLE;
	public MatchRule rule = MatchRule.REACH_TARGET_KILLS;
	
	HudManager hudManager;

	public int targetKills = 5;
	public float timeLimit = 5 * 60.0f;


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
