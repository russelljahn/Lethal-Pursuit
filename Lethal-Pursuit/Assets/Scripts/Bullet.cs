using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour {

	public Spaceship sourceSpaceship;
	public GameObject explosion;

	public Vector3 direction = Vector3.forward;
	public float damage = 15f;
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
			this.transform.position = collision.contacts[0].point;
			alreadyDying = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			HandleApplyingDamage(collision.gameObject);
			GameObject.Destroy(this.gameObject);
		}
	}
	

	bool ShouldExplodeOnContact(GameObject other) {
		return !(other.CompareTag("Bullet") || other == sourceSpaceship.gameObject);
	}


	void HandleApplyingDamage(GameObject hitGameObject) {
		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));
		
		//		Debug.Log ("hitGameObject: " + hitGameObject.name);
		//		Debug.Log ("hitGameObject is IDamageable: " + (damageableObject is IDamageable));
		
		if (damageableObject != null) {
			damageableObject.ApplyDamage(damage, hitGameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}

}
