using UnityEngine;
using System.Collections;

public class HudMessages : MonoBehaviour {


	public SpaceshipMatchData matchData;
	public SpaceshipHealth spaceshipHealth;
	private UILabel label;

	private string [] deathAdjectives = {
		"Grandly",
		"Soundly",
		"Decidably",
		"Gloriously",
		"Softly",
		"Glamorously",
		"Beautifully",
		"Masterfully",
		"Lightly",
		"Deftly",
		"Completely",
		"Effectively",
		"Splendidly",
		"Most excellently",
		"Nicely",
		"Judiciously",
		"Justly",
		"Extensively",
		"Thoroughly",
		"Vigorously",
		"Mightily",
		"Utterly",
		"Happily",
		"Heroically",
		"Tenderly",
		"Mercifully",
		"Tragically",
		"Flamboyantly",
		"Quietly",
		"Marvelously",
		"Perfectly",
		"Swiftly",
		"Undoubtedly",
		"Messily",
		"Dutifully",
		"Gracefully",
		"Elegantly",
		"Stylishly",
	};

	private string [] deathVerbs = {
		"slaughtered",
		"slathered",
		"streamrolled",
		"punked",
//		"slain",
		"annihilated",
		"demolished",
		"shredded",
		"scrapped",
		"derezzed",
		"gibbed",
//		"torn asunder",
//		"sent to the grave",
//		"sent to the afterlife",
		"purged",
		"skelefied",
//		"put to sleep",
		"terminated",
		"liquidated",
		"wasted",
		"put down",
		"eradicated",
		"exterminated",
		"blitzkrieged",
		"erased",
		"executed",
		"ravaged",
		"incinerated",
		"devoured",
		"felled",
		"mashed",
		"mushed",
		"devastated",
//		"scattered to the winds",
		"creamed",
		"toasted",
		"spliced",
		"nuked",
		"vaporized",
		"salted",
		"assassinated",
		"silenced",
		"grilled",
		"gunned down",
		"zapped",
		"torched",
		"emancipated",
		"snuffed out",
		"guillotined",
		"neutralized",
		"crushed",
	};
	private string currentDeathVerb;
	private string currentDeathAdjective;
	

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		matchData = null;
		Random.seed = (int)(Time.time*313751);
		UpdateDeathWords();
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
				if (matchData.lastKilledBy == matchData.spaceship.gameObject) {
					label.text = string.Format("Suicide!\nRespawning in {0}...", (int)matchData.spawnTimeRemaining);
				}
				else {
					label.text = string.Format("{0} {1} by Player {2}!\nRespawning in {3}...", currentDeathAdjective, currentDeathVerb, (spaceshipHealth.lastHurtByPlayerID+1), (int)matchData.spawnTimeRemaining); 
				}
			}
			else if (matchData.spawnTimeRemaining > 0.0f) {
				label.text = string.Format("Preparing for descent..."); 
				UpdateDeathWords();
			}
			else if (MatchManager.lastKilledPlayerId != -1) {
				if (MatchManager.lastKilledPlayerIdLastFrame != MatchManager.lastKilledPlayerId) {
					UpdateDeathWords();
				}
				label.text = string.Format("{0} {1} Player {2}!", currentDeathAdjective, currentDeathVerb, (MatchManager.lastKilledPlayerId+1));
			}
			else {
				label.text = "";
			}
		}
	}

	private void UpdateDeathWords() {
		currentDeathVerb = deathVerbs[Random.Range(0, deathVerbs.Length)];
		currentDeathAdjective = deathAdjectives[Random.Range(0, deathAdjectives.Length)];
	}

}
