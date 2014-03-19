using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {

	public AudioClip pickup1;

	void OnTriggerEnter(Collider collider) {
		
		if (collider.gameObject.CompareTag ("Spaceship")) {
			Debug.Log (this.gameObject.name + "was picked up by: " + collider.gameObject.name);
			audio.PlayOneShot(pickup1);
			
			if(NetworkManager.IsSinglePlayer()) {
				GameObject.Destroy(this.gameObject);
			}
			else {
				networkView.RPC("NetworkDestroyPickup", RPCMode.All);
			}	
		}
		
	}

	[RPC]
	void NetworkDestroyPickup() {
		GameObject.Destroy (this.gameObject);	
	}	

	
}
