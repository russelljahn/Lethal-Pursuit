using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class Collapse : MonoBehaviour {
	
	public float explosionForce;
	public float explosionRadius;

	private bool alreadyTriggered = false;



	void Start() {
		this.collider.isTrigger = true;
	}



	void OnTriggerStay (Collider other) {

		if (!alreadyTriggered && !(other.CompareTag ("IgnoreTrigger") || other.CompareTag ("Unpassable"))) {

//			Debug.Log (other.collider.gameObject.name + " collided!");

			alreadyTriggered = true;

			/* Go through children and add an explosion force to each. */
			for (int i = 0; i < transform.childCount; ++i) {
				Transform child = transform.GetChild(i);
				child.rigidbody.AddExplosionForce(explosionForce, child.position + Random.insideUnitSphere*explosionRadius, explosionRadius);
			}

		}
	}



}
