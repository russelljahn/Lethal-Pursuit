using UnityEngine;
using System.Collections;

public class HudTimeRemaining : MonoBehaviour {


	public SpaceshipMatchData matchData;
	private UILabel label;
	private Level currentLevel;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
		currentLevel = LevelManager.GetLoadedLevel();
		matchData = null;
	}
	
	// Update is called once per frame
	void Update () {

		if (matchData == null) {
			Spaceship ship = GameplayManager.spaceship;
			
			if (ship != null) {
				matchData = GameplayManager.spaceship.GetComponent<SpaceshipMatchData>();
			}
		}
		else {
			label.text = string.Format("Time: {0}", matchData.timeElapsed); 
		}
	}
}
