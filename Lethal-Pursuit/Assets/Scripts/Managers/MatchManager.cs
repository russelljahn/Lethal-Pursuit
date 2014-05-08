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
	
	public static int targetKills = 10;
	public static float timeLimit = 5 * 60.0f;
	
	public int[] killscores;
	public int personalScore = 0;
	
	private Level currentLevel;
	
	public float timeStartMatch = 0.0f;
	public float timeFinishMatch = Mathf.Infinity;
	public float timeElapsed = 0.0f;
	public bool matchOver = false;
	
	public int scoreLeader;

	public static int lastKilledPlayerId;
	public static int lastKilledPlayerIdLastFrame;
	private float timeRemainingForVictim;
	private float maxTimeForVictim = 5.0f;
	
	
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
//			Debug.Log ("Match ended, Winner is Player " + CheckMatchScoreLeader() + " at time '" + timeFinishMatch + "'!");
			matchOver = true;
			networkView.RPC("OnMatchOver", RPCMode.All, CheckMatchScoreLeader());
		}
		
		if (Network.isServer) {
//			for (int i=0; i<killscores.Length; i++) {
//				Debug.Log("Player " + i + " has score " + killscores[i]);
//			}
		}


		// ...
		if (MatchManager.lastKilledPlayerId == MatchManager.lastKilledPlayerIdLastFrame) {
			timeRemainingForVictim = Mathf.Max(0.0f, timeRemainingForVictim-Time.deltaTime);
		}
		else {
			timeRemainingForVictim = maxTimeForVictim;
		}
		if (timeRemainingForVictim == 0.0f) {
			MatchManager.lastKilledPlayerId = -1;
		}
		MatchManager.lastKilledPlayerIdLastFrame = MatchManager.lastKilledPlayerId;
		// ...

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
		Spaceship [] ships = GameObject.FindObjectsOfType<Spaceship>();
		for (int i = 0; i < ships.Length; ++i) {
			ships[i].GetComponent<SpaceshipControl>().enabled = false;
			ships[i].GetComponentInChildren<SpaceshipGun>().enabled = false;
			ships[i].GetComponentInChildren<SpaceshipPickups>().enabled = false;
		}
		for (int i = 0; i < killscores.Length; ++i) {
			killscores[i] = 0;
		}
	}
	
	
	public void OnMatchOverGUI() {
		hudManager.DisplayMatchOver();
	}
	
	
	public void InformServerForKilledBy(int playerID) {
		
		Debug.Log("Killed by player " + playerID);
		
		if(playerID != -1) {
			if (Network.isClient) {
				networkView.RPC("ServerTallyKill", RPCMode.Server, playerID, NetworkManager.GetPlayerID());
			}
			else {
				IncrementScore(playerID, NetworkManager.GetPlayerID());
			}
		}
	}
	
	[RPC]
	public void ServerTallyKill(int killer, int victim) {
		IncrementScore(killer, victim);
	}
	
	public void IncrementScore(int killer, int victim) {
		Debug.Log("Kill tallied for player: " + killer);
		
		killscores[killer]++;
		/*		
		if(killer == 0) {
			personalScore = killscores[killer];
		}
		else {
*/
		Debug.Log("Sending score update to all players with update for " + killer);
		networkView.RPC("UpdatePlayerScores", RPCMode.All, killer, killscores[killer]);

		if (killer == 0) {
			lastKilledPlayerId = victim;
			Debug.Log ("Just killed: " + MatchManager.lastKilledPlayerId);	
		}
		else {
			Debug.Log ("Client about to RPC InformKillerOfVictim() w/ arguments " + NetworkManager.GetPlayerList()[killer] + " and " + victim);
			networkView.RPC("InformKillerOfVictim", NetworkManager.GetPlayerList()[killer], victim);
		}
	}
	
	[RPC]
	public void UpdatePlayerScores(int killer, int score) {
		killscores[killer] = score;
		personalScore = (NetworkManager.GetPlayerID() == killer) ? score : personalScore;
	}
	
	[RPC]
	public void InformKillerOfVictim(int victim) {
		//Do whatever you want here. Info is sent on victim ID.
		lastKilledPlayerId = victim;
//		displayedName = false;
		Debug.Log ("Just killed: " + MatchManager.lastKilledPlayerId);
		
	}

	
	
}
