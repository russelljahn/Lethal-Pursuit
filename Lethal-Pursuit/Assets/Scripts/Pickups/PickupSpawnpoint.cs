using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class PickupSpawnPoint : MonoBehaviour {


	public Pickup pickup; // Should be attached to a separate GameObject.
	public float respawnTime = 10.0f;
	public float timeRemainingUntilRespawn = 0.0f;
	public AudioClip pickupSound;
	public GameObject visuals;

	public bool enabled = true;


	void Start() {
		this.collider.isTrigger = true;
		pickup.gameObject.SetActive(false);
	}


	void Update() {
		timeRemainingUntilRespawn = Mathf.Max (0.0f, timeRemainingUntilRespawn-Time.deltaTime);
		if (!enabled && timeRemainingUntilRespawn == 0.0f) {
			enabled = true;
			visuals.SetActive(true);
		}
	}


	void OnTriggerEnter(Collider collider) {

		Debug.Log (collider.gameObject + " entered spawn area.");
		Debug.Log ("collider.gameObject.CompareTag ('Spaceship')? " + collider.gameObject.CompareTag ("Spaceship")); 

		if (!enabled) {
			return;
		}
		Debug.Log ("Spawn point is enabled!");
		if (collider.gameObject.CompareTag ("Spaceship")) {
			Debug.Log ("Can Pickup? " + collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup));
		}

		if (collider.gameObject.CompareTag ("Spaceship") && collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup)) {
//			Debug.Log (this.gameObject.name + " was picked up by: " + collider.gameObject.name);
			audio.PlayOneShot(pickupSound);

			GameObject pickupClone = GameObject.Instantiate(pickup.gameObject) as GameObject;
			collider.gameObject.GetComponent<SpaceshipPickups>().GetPickup(pickupClone.GetComponent<Pickup>());
			
			if (NetworkManager.IsSinglePlayer()) {
				enabled = false;
				visuals.SetActive(false);
				timeRemainingUntilRespawn = respawnTime;
			}
			else {
				networkView.RPC("NetworkHidePickupSpawnPoint", RPCMode.All);
			}	
		}
		
	}


	void OnTriggerExit(Collider collider) {
		Debug.Log (collider.gameObject + " left spawn area.");
	}


	[RPC]
	void NetworkHidePickupSpawnPoint() {
		enabled = false;
		visuals.SetActive(false);
		timeRemainingUntilRespawn = respawnTime;
	}

	
}
