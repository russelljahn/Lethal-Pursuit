using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Checkpoint : MonoBehaviour {

	private CheckpointManager checkpointManager;
	public int id; // Assigned by Checkpoint Manager

	void Awake() {
		this.gameObject.tag = "Checkpoint";
	}



	// Use this for initialization
	void Start () {
		checkpointManager = CheckpointManager.Get();
	}


	
	// Update is called once per frame
	void Update () {
	
	}



	void OnTriggerEnter(Collider collider) {

		Debug.Log ("Collided with checkpoint!");

		if (collider.gameObject.CompareTag("Spaceship")) {
			checkpointManager.OnShipEnterCheckpoint(collider.GetComponent<Spaceship>(), this);
		}

	}

}
