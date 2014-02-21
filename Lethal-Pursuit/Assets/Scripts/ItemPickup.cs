using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public AudioClip pickup1;

	void OnTriggerEnter(Collider collider) {
		
		if (collider.gameObject.CompareTag ("Spaceship")) {
			Debug.Log (this.gameObject.name + "was picked up by: " + collider.gameObject.name);
			audio.PlayOneShot(pickup1);
			GameObject.Destroy(this.gameObject);

		}
		
	}

		

	
}
