using UnityEngine;
using System.Collections;

public class HudDeath : MonoBehaviour {


	public SpaceshipMatchData matchData;
	public SpaceshipHealth spaceshipHealth;
	private UILabel label;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		matchData = null;
	}
	
	// Update is called once per frame
	void Update () {

		if (matchData == null) {
			Spaceship ship = GameplayManager.spaceship;
			
			if (ship != null) {
				matchData = GameplayManager.spaceship.GetComponent<SpaceshipMatchData>();
				spaceshipHealth = GameplayManager.spaceship.GetComponent<SpaceshipHealth>();
			}
		}
		else {
			if (matchData.spawnTimeRemaining > 1.0f) { 
				label.text = string.Format("Slain by Player {0}!\nRespawning in {1}...", (spaceshipHealth.lastHurtByPlayerID+1), (int)matchData.spawnTimeRemaining); 
			}
			else if (matchData.spawnTimeRemaining > 0.0f) {
				label.text = string.Format("Preparing for descent..."); 
			}
			else {
				label.text = "";
			}
		}
	}
}
