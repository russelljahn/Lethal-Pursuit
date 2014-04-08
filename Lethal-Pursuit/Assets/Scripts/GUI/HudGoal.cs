using UnityEngine;
using System.Collections;

public class HudGoal : MonoBehaviour {
	
	private UILabel label;
	public MatchManager matchManager;
		
	void Start() {
		label = GetComponent<UILabel>();
	}


	void Update() {
//		int kills =	networkView.RPC("GetKills", RPCMode.Server, NetworkManager.GetPlayerID());
		label.text = string.Format("{0}/{1}", matchManager.personalScore, MatchManager.targetKills);
	}

}