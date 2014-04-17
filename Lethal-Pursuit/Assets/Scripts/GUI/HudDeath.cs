using UnityEngine;
using System.Collections;

public class HudDeath : MonoBehaviour {


	public SpaceshipMatchData matchData;
	public SpaceshipHealth spaceshipHealth;
	private UILabel label;

	private string [] deathMessages = {
		"Slaughtered by Player {0}!",
		"Slathered by Player {0}!",
		"Streamrolled by Player {0}!",
		"Punked by Player {0}!",
		"Slain by Player {0}!",
		"Annihilated by Player {0}!",
		"Demolished by Player {0}!",
		"Shredded by Player {0}!",
		"Scrapped by Player {0}!",
		"Rezzed by Player {0}!",
		"Gibbed by Player {0}!",
		"Torn asunder by Player {0}!",
		"Sent to the grave by Player {0}!",
		"Sent to the afterlife by Player {0}!",
		"Purged by Player {0}!",
		"Skelefied by Player {0}!",
		"Put to sleep by Player {0}!",
		"Terminated by Player {0}!",
		"Liquidated by Player {0}!",
		"Wasted by Player {0}!",
		"Put down by Player {0}!",
		"Eradicated by Player {0}!",
		"Exterminated by Player {0}!",
	};
	private string currentDeathMessage;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		matchData = null;
		Random.seed = (int)(Time.time*313751);
		currentDeathMessage = deathMessages[Random.Range(0, deathMessages.Length)];
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
				label.text = string.Format((currentDeathMessage + "\nRespawning in {1}..."), (spaceshipHealth.lastHurtByPlayerID+1), (int)matchData.spawnTimeRemaining); 
			}
			else if (matchData.spawnTimeRemaining > 0.0f) {
				label.text = string.Format("Preparing for descent..."); 
				currentDeathMessage = deathMessages[Random.Range(0, deathMessages.Length)];
			}
			else {
				label.text = "";
			}
		}
	}
}
