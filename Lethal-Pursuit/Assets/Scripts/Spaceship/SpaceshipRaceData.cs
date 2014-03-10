using UnityEngine;
using System.Collections;

public class SpaceshipRaceData : SpaceshipComponent {

	private Level currentLevel;

	public int lapsCompleted = 0;
	public bool finishedTrack = false;

	private int numCheckpoints;
	public Checkpoint lastCheckpoint;

	public float startRaceTime = 0.0f;
	public float finishRaceTime = Mathf.Infinity;

	public float finishLapTime = Mathf.Infinity;
	public float timeElapsed;

	private RaceManager raceManager;
	


	void Start () {
		lastCheckpoint = Checkpoint.GetCheckpointByID(0);
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
				lastCheckpoint = checkpoint;
			}
			else if (HaveLapped(checkpoint)) {
				lastCheckpoint = checkpoint;
				++lapsCompleted;
				finishLapTime = timeElapsed;
				raceManager.SendMessage("OnFinishLap", this);
			}

			if (FinishedRace()) {
				lastCheckpoint = checkpoint;
				Debug.Log (this.gameObject.name + " finished race!");
				finishRaceTime = timeElapsed;
				finishedTrack = true;
				raceManager.SendMessage("OnFinishRace", this); 
			}
		}
		
	}


	bool IsNextCheckpoint(Checkpoint checkpoint) {
		return lastCheckpoint == null || checkpoint.id == lastCheckpoint.id+1;
	}


	bool HaveLapped(Checkpoint checkpoint) {
		return checkpoint.id == 0 && lastCheckpoint.id == numCheckpoints-1;
	}


	public bool FinishedRace() {
		return lapsCompleted >= currentLevel.lapsToWin;
	}




}
