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



	void OnCollisionEnter(Collision collision) {

		if (collision.collider.gameObject.CompareTag("Spaceship")) {
			checkpointManager.OnShipEnterCheckpoint(collision.collider.GetComponent<Spaceship>(), this);
		}

	}

}
