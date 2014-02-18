using UnityEngine;
using System.Collections;

public struct CheckpointData {
	Spaceship spaceship;
	int lastCheckpoint;
	int lapsCompleted;
}


public class CheckpointManager : MonoBehaviour {

	private static CheckpointManager singletonInstance = null;
	public Checkpoint [] checkpoints;

	


	void Awake() {
		this.gameObject.tag = "CheckpointManager";
	}



	void Start () {
		AssignCheckpointIDs();
	}



	void Update () {
	
	}



	void AssignCheckpointIDs() {
		for (int i = 0; i < checkpoints.Length; ++i) {
			if (checkpoints[i] == null) {
				continue;
			}
			checkpoints[i].id = i;
		}
	}



	public static CheckpointManager Get() {
		/* If first time accessing instance, then find it... */
		if (singletonInstance == null) {
			singletonInstance = FindObjectOfType(typeof (CheckpointManager)) as CheckpointManager;
		}
		
		/* If instance is null, then no CheckpointManager exists in the scene, so create one. */
		if (singletonInstance == null) {
			GameObject obj = new GameObject("CheckpointManager");
			singletonInstance = obj.AddComponent(typeof (CheckpointManager)) as CheckpointManager;
			obj.name = "CheckpointManager";
		}
		return singletonInstance;
	}
	
	
	
	
	
	
	
	public void OnShipEnterCheckpoint(Spaceship spaceship, Checkpoint checkpoint) {
		Debug.Log ("spaceship '" + spaceship.gameObject.name + "' entered checkpoint '" + checkpoint.gameObject.name + "'!");
	}
}
