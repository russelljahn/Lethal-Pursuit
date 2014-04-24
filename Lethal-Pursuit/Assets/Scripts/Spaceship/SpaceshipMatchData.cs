using UnityEngine;
using System;
using System.Collections;

public class SpaceshipMatchData : SpaceshipComponent {
	
	private Level currentLevel;
	
	public float timeStartMatch = 0.0f;
	public float timeFinishMatch = Mathf.Infinity;
	public float timeElapsed = 0.0f;
	public bool matchOver = false;
	
	private MatchManager matchManager;
	public int totalKills = 0;

	public float spawnTimeRemaining = 0.0f;
	public GameObject lastKilledBy;

	
	
/*
	public override void Start () {
		base.Start();

		matchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();

		timeStartMatch = Time.time;
		currentLevel = LevelManager.GetLoadedLevel();
	}
*/
//	public override void Update() {
//		base.Update();
//
//		if (MatchManager.lastKilledPlayerIdLastFrame == MatchManager.lastKilledPlayerIdLastFrame) {
//			timeRemainingForVictim = Mathf.Max(0.0f, timeRemainingForVictim-Time.deltaTime);
//		}
//		else {
//			timeRemainingForVictim = maxTimeForVictim;
//		}
//
//		if (timeRemainingForVictim == 0.0f) {
//			MatchManager.lastKilledPlayerId = -1;
//		}
//
//		MatchManager.lastKilledPlayerIdLastFrame = MatchManager.lastKilledPlayerId;
//	}

	
/*
		public bool IsMatchOver() {
		switch (matchManager.rule) {
			case MatchRule.REACH_TARGET_KILLS:
				return totalKills >= matchManager.targetKills;

			default:
				throw new Exception("IsMatchOver(): Unknown gameplay mode: " + matchManager.rule);
		}
		return false;
}
*/
	
}