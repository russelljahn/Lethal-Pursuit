using UnityEngine;
using System.Collections;

public class SpaceshipRaceData : SpaceshipComponent {

	private Level currentLevel;

	public int lapsCompleted = 0;
	public bool finishedTrack = false;

	private int numCheckpoints;
	public int lastCheckpointId;

	public float startRaceTime = 0.0f;
	public float finishRaceTime = Mathf.Infinity;

	public float finishLapTime = Mathf.Infinity;
	public float timeElapsed;

	private RaceManager raceManager;
	


	void Start () {
		numCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
		raceManager = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();

		startRaceTime = Time.time;
		currentLevel = LevelManager.GetLoadedLevel();
	}



	void Update() {
		timeElapsed += Time.deltaTime;
	}



	void OnTriggerEnter(Collider collider) {

		Debug.Log ("Spaceship triggered by: " + collider.gameObject.name);

		if (collider.gameObject.CompareTag("Checkpoint")) {

			if (FinishedRace()) {
				return;
			}

			Checkpoint checkpoint = collider.gameObject.GetComponent<Checkpoint>();

			if (IsNextCheckpoint(checkpoint)) {
				lastCheckpointId = checkpoint.id;
			}
			else if (HaveLapped(checkpoint)) {
				lastCheckpointId = 0;
				++lapsCompleted;
				finishLapTime = timeElapsed;
				raceManager.SendMessage("OnFinishLap", this); 
				// Should announce completed a lap? Figure out if we need to.
			}

			if (FinishedRace()) {
				Debug.Log (this.gameObject.name + " finished race!");
				finishRaceTime = timeElapsed;
				finishedTrack = true;
				raceManager.SendMessage("OnFinishRace", this); 
			}
		}
		
	}


	bool IsNextCheckpoint(Checkpoint checkpoint) {
		return checkpoint.id == lastCheckpointId+1;
	}


	bool HaveLapped(Checkpoint checkpoint) {
		return checkpoint.id == 0 && lastCheckpointId == numCheckpoints-1;
	}


	public bool FinishedRace() {
		return lapsCompleted >= currentLevel.lapsToWin;
	}

}
