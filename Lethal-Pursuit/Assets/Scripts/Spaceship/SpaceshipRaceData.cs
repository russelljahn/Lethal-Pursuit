using UnityEngine;
using System.Collections;

public class SpaceshipRaceData : SpaceshipComponent {


	public int lapsCompleted = 0;
	private static int numCheckpoints;

	public int lastCheckpointId;



	void Start () {
		// Cache # of checkpoints
		if (numCheckpoints == 0) {
			numCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
		}
	}
	



	void OnTriggerEnter(Collider collider) {

		Debug.Log ("Spaceship triggered by: " + collider.gameObject.name);

		if (collider.gameObject.CompareTag("Checkpoint")) {

			Checkpoint checkpoint = collider.gameObject.GetComponent<Checkpoint>();

			if (IsNextCheckpoint(checkpoint)) {
				lastCheckpointId = checkpoint.id;
			}
			else if (HaveLapped(checkpoint)) {
				lastCheckpointId = 0;
				++lapsCompleted;
			}
		}
		
	}


	bool IsNextCheckpoint(Checkpoint checkpoint) {
		return checkpoint.id == lastCheckpointId+1;
	}


	bool HaveLapped(Checkpoint checkpoint) {
		return checkpoint.id == 0 && lastCheckpointId == numCheckpoints-1;
	}

}
