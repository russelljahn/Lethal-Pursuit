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

	public int[] killscores;

	private Level currentLevel;
	
	public float timeStartMatch = 0.0f;
	public float timeFinishMatch = Mathf.Infinity;
	public float timeElapsed = 0.0f;
	public bool matchOver = false;

	public int scoreLeader;


	// Use this for initialization
	void Start () {
		hudManager = GameObject.FindGameObjectWithTag("HudManager").GetComponent<HudManager>();
		killscores = new int[NetworkManager.GetPlayerList().Count];
		
		if(Network.isServer) {
			timeStartMatch = Time.time;
			networkView.RPC("SetStartTime", RPCMode.AllBuffered, timeStartMatch);
		}
		currentLevel = LevelManager.GetLoadedLevel();
	}


	[RPC]
	public void SetStartTime(float startTime) {
		timeStartMatch = startTime;
		timeElapsed = 0.0f;
		Debug.Log("Start Time given from server: " + timeStartMatch);
	}	

	
	void Update() {

		scoreLeader = CheckMatchScoreLeader();
		
		if (Network.isServer) {
			timeElapsed += Time.deltaTime;
		}
		
		if (IsMatchOver() && !matchOver) {
			timeFinishMatch = timeStartMatch + timeElapsed;
			Debug.Log ("Match ended, Winner is Player " + CheckMatchScoreLeader() + " at time '" + timeFinishMatch + "'!");
			matchOver = true;
			networkView.RPC("OnMatchOver", RPCMode.All, CheckMatchScoreLeader());
		}

		if (Network.isServer) {
			for (int i=0; i<killscores.Length; i++) {
				Debug.Log("Player " + i + " has score " + killscores[i]);
			}
		}
	}

	
	public bool IsMatchOver() {
		if (NetworkManager.IsSinglePlayer()) {
			return false;
		}
		switch (rule) {
			case MatchRule.REACH_TARGET_KILLS:
				//Update GUI values here
				
				return killscores[scoreLeader] >= targetKills;
				
			default:
				throw new Exception("IsMatchOver(): Unknown gameplay mode: " + rule);
		}
		return false;
	}


	private int CheckMatchScoreLeader() {
		int playerID = 0;
		int bestScore = 0;

		for (int i=0; i<killscores.Length; i++) {
			if (killscores[i] > bestScore) {
				playerID = i;
				bestScore = killscores[i];
			}
		}

		return playerID; 
	}


	[RPC]
	public void OnMatchOver(int winnerID) {
		OnMatchOverGUI();
		scoreLeader = winnerID;
		for (int i = 0; i < killscores.Length; ++i) {
			killscores[i] = 0;
		}
	}


	public void OnMatchOverGUI() {
		hudManager.DisplayMatchOver();
	}


	public void InformServerForKilledBy(int playerID) {
		if (Network.isClient) {
			networkView.RPC("ServerTallyKill", RPCMode.Server, playerID);
		}
		else {
			killscores[playerID]++;
		}
	}


	[RPC]
	public void ServerTallyKill(int playerID) {
		Debug.Log("Kill tallied for player: " + playerID);
		++killscores[playerID];
		networkView.RPC ("ClientTallyKill", RPCMode.OthersBuffered, playerID);
	}









}
