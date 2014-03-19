using UnityEngine;
using System.Collections;

public class HUD_Laps : MonoBehaviour {


	public SpaceshipRaceData raceData;
	private UILabel label;
	private Level currentLevel;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		currentLevel = LevelManager.GetLoadedLevel();
		raceData = null;
	}
	
	// Update is called once per frame
	void Update () {

		if(raceData == null) {
			Spaceship ship = GameplayManager.spaceship;
			
			if(ship != null) {
				raceData = GameplayManager.spaceship.GetComponent<SpaceshipRaceData>();
			}
		}
		else {
			Debug.Log("raceData " + (raceData));
			Debug.Log("currentLevel " + (currentLevel));
			label.text = string.Format("Lap {0}/{1}", raceData.lapsCompleted+1, currentLevel.lapsToWin); 
		}
	}
}
