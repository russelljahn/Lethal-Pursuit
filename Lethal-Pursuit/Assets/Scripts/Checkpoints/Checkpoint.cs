using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Checkpoint : MonoBehaviour {
	
	public int id; 



	void Awake() {
		this.gameObject.tag = "Checkpoint";
	}




}
