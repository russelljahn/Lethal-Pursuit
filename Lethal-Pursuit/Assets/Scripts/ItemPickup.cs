using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public AudioClip pickup1;

	void OnCollisionEnter(Collision collision) {
		Debug.Log (this.gameObject.name + "was touched by: " + collision.gameObject.name + " with tag: " + collision.gameObject.tag);

		
		if (collision.gameObject.CompareTag ("Spaceship")) {
			Debug.Log (this.gameObject.name + "was picked up by: " + collision.gameObject.name);
			audio.PlayOneShot(pickup1);
			GameObject.Destroy(this.gameObject);

		}
		
	}

		
	void OnCollisionStay(Collision collision) {
		Debug.Log("OnCollisionStay");
	}


	
}
