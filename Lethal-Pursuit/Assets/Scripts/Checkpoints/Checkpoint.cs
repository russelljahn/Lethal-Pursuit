using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Checkpoint : MonoBehaviour {
	
	public int id; 



	void Awake() {
		this.gameObject.tag = "Checkpoint";
	}



	public static Checkpoint GetCheckpointByID(int id) {
		Checkpoint [] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
		for (int i = 0; i < checkpoints.Length; ++i) {
			if (checkpoints[i].id == id) {
				return checkpoints[i];
			}
		}
		throw new Exception("No checkpoint with id '" + id + "'");
	}




}
