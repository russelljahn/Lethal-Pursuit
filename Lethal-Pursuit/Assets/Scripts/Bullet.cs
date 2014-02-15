using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 direction;
	public float speed = 50f;
	public float timeUntilDeath = 5.0f;
//	public bool prefab = true;
	public bool alreadyActivatedOnDeath = false;

	public Detonator detonater;

	// Use this for initialization
	void Start () {
//		Debug.Log ("Bullet.transform.position: " + this.transform.position);
		direction.Normalize();
	}




	
	// Update is called once per frame
	void Update () {

//		if (!prefab) {
			timeUntilDeath -= Time.deltaTime;
			if (timeUntilDeath <= 0 && !alreadyActivatedOnDeath) {
				alreadyActivatedOnDeath = true;
				StartCoroutine(OnDeath());
			}

			this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
//		}
	}


	IEnumerator OnDeath () {
		GameObject.Destroy(this.gameObject);
		yield break;
	}


	void OnCollisionEnter(Collision collision) {
		if (!alreadyActivatedOnDeath && ShouldExplodeOnContact(collision.gameObject)) {
			Debug.Log (this.gameObject.name + "collided with: " + collision.gameObject.name);
			
			alreadyActivatedOnDeath = true;
			detonater.Explode();
			StartCoroutine(OnDeath());
		}

	}


	bool ShouldExplodeOnContact(GameObject gameObject) {
		return !(gameObject.CompareTag("Bullet") || gameObject.CompareTag("Spaceship"));
	}

}
