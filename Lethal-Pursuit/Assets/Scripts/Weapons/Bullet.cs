using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Spaceship sourceSpaceship;
	public GameObject explosion;

	public Vector3 direction;
	public float speed = 50f;
	public float timeUntilDeath = 5.0f;
	private bool alreadyDying = false;

	
	void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;
		if (timeUntilDeath <= 0 && !alreadyDying) {
			alreadyDying = true;
			GameObject.Destroy(this.gameObject);
		}
		this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
	}


	void OnCollisionEnter(Collision collision) {
		if (!alreadyDying && ShouldExplodeOnContact(collision.gameObject)) {
//			Debug.Log (this.gameObject.name + "collided with: " + collision.gameObject.name);
			alreadyDying = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			GameObject.Destroy(this.gameObject);
		}
	}
	

	bool ShouldExplodeOnContact(GameObject other) {
		return !(other.CompareTag("Bullet") || other == sourceSpaceship.gameObject);
	}

}
