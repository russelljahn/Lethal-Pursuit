using UnityEngine;
using System.Collections;

public class TutorialDriftEvent : MonoBehaviour {

	public TutorialManager tutorialManager;
	public Spaceship spaceship;
	bool firstTime = true;


//
//	void OnCollisionEnter(Collision collision) {
//		Debug.Log ("OhgdfiughOUHIUAHGIUFH!!!!");
//		if (firstTime) {
//			tutorialManager.enteredTeachDriftZone = true;
//			firstTime = false;
//		}
//	}


	void Start() {
		spaceship = GameplayManager.spaceship;
	}



	void Update () {
		if (firstTime && Vector3.Distance(spaceship.transform.position, this.transform.position) < 500.0f) {
			tutorialManager.enteredTeachDriftZone = true;
			firstTime = false;
		}
	}


}
