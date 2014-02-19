using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

	public Spaceship spaceship;
	public UILabel label;
	public UI2DSprite overlay;


	private enum TutorialState {
		TEACH_BOOST,
		TEACH_DRIFT,
		TEACH_SHOOT,
		TEACH_PICKUPS
	};


	private TutorialState [] states = {
		TutorialState.TEACH_BOOST
	};

	private TutorialState currentState;


	// Use this for initialization
	void Start () {
		currentState = states[0];
		overlay.gameObject.SetActive(true);
		label.text = "Press R2 to boost!";
	}
	
	// Update is called once per frame
	void Update () {
		if (currentState == TutorialState.TEACH_BOOST && spaceship.boosting) {
			overlay.gameObject.SetActive(false);
		}
	}
}
