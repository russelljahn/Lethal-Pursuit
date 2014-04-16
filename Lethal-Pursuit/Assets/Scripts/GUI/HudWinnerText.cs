using UnityEngine;
using System.Collections;

public class HudWinnerText : MonoBehaviour {

	UILabel winnerText;
	public MatchManager matchManager;

	// Use this for initialization
	void Start () {
		winnerText = GetComponent<UILabel>();
		if (NetworkManager.GetPlayerID() == matchManager.scoreLeader) {
			winnerText.text = string.Format("YOU ARE THE VICTOR!");
		}
		else {
			winnerText.text = string.Format("PLAYER {0} WINS", (matchManager.scoreLeader+1));
		}
	}

}
