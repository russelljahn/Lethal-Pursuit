using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

	private Spaceship spaceship;
	public UILabel label;
	public UI2DSprite overlay;


	private enum TutorialState {
		TEACH_BOOST,
		TEACH_DRIFT,
		TEACH_SHOOT,
		TEACH_PICKUPS,
		DONE
	};


	private TutorialState [] states = {
		TutorialState.TEACH_BOOST
	};

	private TutorialState currentState;

	public bool enteredTeachDriftZone = false;
	public bool exitedTeachDriftZone = false;
	


	// Use this for initialization
	void Start () {
		if (!LevelManager.GetLoadedLevel().name.Equals("Tutorial")) {
			label.gameObject.SetActive(false);
			overlay.enabled = false;
			this.gameObject.SetActive(false);
		}
		currentState = states[0];
		overlay.gameObject.SetActive(true);
		label.text = "Press R2 to boost!";
		spaceship = GameplayManager.spaceship;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentState == TutorialState.TEACH_BOOST && spaceship.boosting) {
			overlay.gameObject.SetActive(false);
			currentState = TutorialState.TEACH_DRIFT;
		}
		else if (enteredTeachDriftZone) {
			overlay.gameObject.SetActive(true);
			label.text = "Hold L2 when turning to drift!";
			currentState = TutorialState.TEACH_DRIFT;
			enteredTeachDriftZone = false;
		}

		if (currentState == TutorialState.TEACH_DRIFT && spaceship.drifting) {
			overlay.gameObject.SetActive(false);
			currentState = TutorialState.DONE;
			
		}
	}
}
