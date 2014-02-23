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
	}
	
	// Update is called once per frame
	void Update () {
		label.text = string.Format("Lap {0}/{1}", raceData.lapsCompleted+1, currentLevel.lapsToWin); 
	}
}
