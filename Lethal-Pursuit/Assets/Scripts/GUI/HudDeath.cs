using UnityEngine;
using System.Collections;

public class HudDeath : MonoBehaviour {


	public SpaceshipMatchData matchData;
	public SpaceshipHealth spaceshipHealth;
	private UILabel label;

	private string [] deathMessages = {
		"Slaughtered",
		"Slathered",
		"Streamrolled",
		"Punked",
		"Slain",
		"Annihilated",
		"Demolished",
		"Shredded",
		"Scrapped",
		"Rezzed",
		"Gibbed",
		"Torn asunder",
		"Sent to the grave",
		"Sent to the afterlife",
		"Purged",
		"Skelefied",
		"Put to sleep",
		"Terminated",
		"Liquidated",
		"Wasted",
		"Put down",
		"Eradicated",
		"Exterminated",
		"Blitzkrieged",
		"Erased",
		"Executed",
		"Ravaged",
		"Incinerated",
		"Devoured",
		"Felled",
		"Mashed",
		"Mushed",
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
				label.text = string.Format("{0} by Player {1}!\nRespawning in {2}...", currentDeathMessage, (spaceshipHealth.lastHurtByPlayerID+1), (int)matchData.spawnTimeRemaining); 
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
