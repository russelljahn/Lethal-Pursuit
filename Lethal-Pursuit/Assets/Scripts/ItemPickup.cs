using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public AudioClip pickup1;

	void OnCollisionEnter(Collision collision) {
		
		if (collision.gameObject.CompareTag ("Spaceship")) {
			Debug.Log (this.gameObject.name + "was picked up by: " + collision.gameObject.name);
			audio.PlayOneShot(pickup1);
			GameObject.Destroy(this);

		}
		
	}


	
}
