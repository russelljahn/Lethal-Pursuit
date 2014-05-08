using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class PickupSpawner : MonoBehaviour {


	public string pathToPickupResource;
	private Pickup pickup;

	public float respawnTime = 10.0f;
	public float timeRemainingUntilRespawn = 0.0f;
	public AudioClip pickupSound;
	public GameObject visuals;

	public new bool enabled = true;


	void Start() {
		this.collider.isTrigger = true;
		pickup = (GameObject.Instantiate(Resources.Load(pathToPickupResource)) as GameObject).GetComponent<Pickup>();
		pickup.transform.parent = this.transform;
		pickup.transform.localPosition = Vector3.zero;
	}


	void Update() {
		timeRemainingUntilRespawn = Mathf.Max (0.0f, timeRemainingUntilRespawn-Time.deltaTime);
		if (!enabled && timeRemainingUntilRespawn == 0.0f) {
			enabled = true;
			visuals.SetActive(true);
		}
	}


	void OnTriggerEnter(Collider collider) {

//		Debug.Log (collider.gameObject + " entered spawn area.");
//		Debug.Log ("collider.gameObject.CompareTag ('Spaceship')? " + collider.gameObject.CompareTag ("Spaceship")); 
//		Debug.Log ("enable? " + enabled);
		if (!enabled || !collider.gameObject.CompareTag ("Spaceship")) {
			return;
		}
		Debug.Log ("Can " + collider.gameObject.name + " pickup " + this.gameObject.name + "? " + collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup));
		
//		Debug.Log ("Spawn point is enabled!");

//		Debug.Log ("collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup): " + collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup));
		if (collider.gameObject.CompareTag ("Spaceship") && collider.gameObject.GetComponent<SpaceshipPickups>().CanPickup(pickup)) {
			Debug.Log (this.gameObject.name + " was picked up by: " + collider.gameObject.name);
			audio.PlayOneShot(pickupSound);

			GameObject pickupClone;
			if (NetworkManager.IsSinglePlayer()) {
				pickupClone = GameObject.Instantiate(pickup.gameObject) as GameObject;
			}
			else {
				pickupClone = (Network.Instantiate(
					Resources.Load(pathToPickupResource),
				    Vector3.zero, 
				    Quaternion.identity, 
				    667) as GameObject
				);
			}
//			Debug.Log ("Pickup clone: " + pickupClone);
//			Debug.Log("collider.gameObject.GetComponent<SpaceshipPickups>(): " + collider.gameObject.GetComponent<SpaceshipPickups>());
			collider.gameObject.GetComponent<SpaceshipPickups>().GetPickup(pickupClone.GetComponent<Pickup>());
			enabled = false;
			visuals.SetActive(false);
			timeRemainingUntilRespawn = respawnTime;

		}
		
	}


	void OnTriggerExit(Collider collider) {
		Debug.Log (collider.gameObject + " left spawn area.");
	}

	
}
