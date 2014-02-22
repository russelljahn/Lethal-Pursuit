using UnityEngine;
using System.Collections;

public class HUD_Laps : MonoBehaviour {


	public SpaceshipRaceData raceData;
	private UILabel label;

	// Use this for initialization
	void Start () {
		label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		label.text = string.Format("Lap {0}/?", raceData.lapsCompleted); 
	}
}
