using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HudScoreboard : MonoBehaviour {
	
	private UILabel label;
	private int numPlayers;
	private List <int> playerRankings;
	private string scoreText;

	private MatchManager matchManager;

	// Use this for initialization
	void Start () {
		matchManager = GameObject.FindWithTag("MatchManager").GetComponent<MatchManager>();
		label = GetComponent<UILabel>();

		numPlayers = NetworkManager.GetPlayerList().Count;
		playerRankings = new List<int>();
		for (int i = 0; i < numPlayers; ++i) {
			playerRankings.Add(i);
		}

	}
	
	// Update is called once per frame
	void Update () {
		/* Sort player ids by ranking. */
		playerRankings.Sort( 
			(int lhs, int rhs) => matchManager.killscores[lhs].CompareTo(matchManager.killscores[rhs]) 
		);

		scoreText = "";
		for (int i = 0; i < numPlayers; ++i) {
			int playerId = playerRankings[i];
			scoreText += string.Format("P{0}: {1}/{2}", playerId, matchManager.killscores[playerId], MatchManager.targetKills);
		}

		label.text = scoreText;
	}

}
