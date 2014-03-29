using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class PickupSpawnPoint : MonoBehaviour {


	Pickup pickup;
	public float respawnTime = 10.0f;
	private float timeRemainingUntilRespawn = 0.0f;
	public AudioClip pickupSound;
	public GameObject visuals;


	void Start() {
		this.collider.isTrigger = true;
	}


	void Update() {
		timeRemainingUntilRespawn = Mathf.Max (0.0f, timeRemainingUntilRespawn-Time.deltaTime);
		if (!gameObject.activeInHierarchy && timeRemainingUntilRespawn == 0.0f) {
			collider.enabled = true;
			visuals.SetActive(true);
		}
	}


	void OnTriggerEnter(Collider collider) {
		
		if (collider.gameObject.CompareTag ("Spaceship") && collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup)) {
			Debug.Log (this.gameObject.name + "was picked up by: " + collider.gameObject.name);
			audio.PlayOneShot(pickupSound);

			collider.gameObject.GetComponent<SpaceshipPickups>().GetPickup(GameObject.Instantiate(pickup.gameObject) as Pickup);
			
			if (NetworkManager.IsSinglePlayer()) {
				collider.enabled = false;
				visuals.SetActive(false);
				timeRemainingUntilRespawn = respawnTime;
			}
			else {
				networkView.RPC("NetworkHidePickupSpawnPoint", RPCMode.All);
			}	
		}
		
	}


	[RPC]
	void NetworkHidePickupSpawnPoint() {
		collider.enabled = false;
		visuals.SetActive(false);
		timeRemainingUntilRespawn = respawnTime;
	}

	
}
