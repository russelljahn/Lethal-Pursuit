using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 direction;

	public float speed = 50f;

	public float timeUntilDeath = 5.0f;
	public bool alreadyActivatedOnDeath = false;

	public Spaceship sourceSpaceship;

	public GameObject explosion;


	// Use this for initialization
	void Start () {
//		direction.Normalize();
	}




	
	// Update is called once per frame
	void Update () {

		timeUntilDeath -= Time.deltaTime;
		if (timeUntilDeath <= 0 && !alreadyActivatedOnDeath) {
			alreadyActivatedOnDeath = true;
			StartCoroutine(OnDeath());
		}

		this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
	}


	IEnumerator OnDeath () {
		GameObject.Destroy(this.gameObject);
		yield break;
	}


	void OnCollisionEnter(Collision collision) {

		if (!alreadyActivatedOnDeath && ShouldExplodeOnContact(collision.gameObject)) {
			Debug.Log (this.gameObject.name + "collided with: " + collision.gameObject.name);
			
			alreadyActivatedOnDeath = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			StartCoroutine(OnDeath());
		}

	}
	

	bool ShouldExplodeOnContact(GameObject other) {
		return !(
			other.CompareTag("Bullet") || 
			other == sourceSpaceship.gameObject
		);
	}

}
