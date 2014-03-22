using UnityEngine;
using System.Collections;

public class SpaceshipMatchData : SpaceshipComponent {

	private Level currentLevel;

	public float timeStartMatch = 0.0f;
	public float timeFinishMatch = Mathf.Infinity;
	public float timeElapsed = 0.0f;
	public bool matchOver = false;

	private MatchManager matchManager;
	


	public override void Start () {
		base.Start();

		matchManager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();

		timeStartMatch = Time.time;
		currentLevel = LevelManager.GetLoadedLevel();
	}



	public override void Update() {
		base.Update();
		timeElapsed += Time.deltaTime;

		if (IsMatchOver()) {
			timeFinishMatch = timeStartMatch + timeElapsed;
			Debug.Log ("Match ended for '" + this.spaceship + " at time '" + timeFinishMatch + "'!");
			matchOver = true;
			matchManager.SendMessage("OnMatchOver", this); 
		}
	}

	

	public bool IsMatchOver() {
		return timeElapsed >= 60.0f;
	}




}
