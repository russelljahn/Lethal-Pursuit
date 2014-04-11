using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class Missile : MonoBehaviour {
	
	public Spaceship sourceSpaceship;
	public GameObject explosion;
	public MeshRenderer missileModel;

	public Vector3 direction = Vector3.forward;
	public float damage = 15f;
	public float speed = 50f;
	public float timeUntilDeath = 5.0f;
	private bool alreadyDying = false;

	public float maxExplosionRadius = 300.0f;
	public float timeUntilMaxExplosionRadius = 0.05f;


	void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;
		if (!alreadyDying && timeUntilDeath <= 0) {
			alreadyDying = true;
			StartCoroutine(Explode());
		}

		if (!alreadyDying) {
			this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
		}
	}


	void OnCollisionEnter(Collision collision) {
		// Ignore explosion if spaceship just shot missile and is within the collider
		if (!alreadyDying && collision.gameObject == sourceSpaceship.gameObject) {
			return;
		}
		if (!alreadyDying && ShouldExplodeOnContact(collision.gameObject)) {
			this.transform.position = collision.contacts[0].point;
			alreadyDying = true;
			StartCoroutine(Explode());
		}
		HandleApplyingDamage(collision.gameObject);
	}


	bool ShouldExplodeOnContact(GameObject other) {
//		Debug.Log ("other == sourceSpaceship.gameObject? " + (other == sourceSpaceship.gameObject));
		return !(other.CompareTag("Bullet") || other == sourceSpaceship.gameObject);
	}


	void HandleApplyingDamage(GameObject hitGameObject) {
		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));

		if (damageableObject != null) {
			// Eventually should have some damage falloff.
//			float damageToApply = 1.0f/(1.0f+Vector3.Distance(this.gameObject.transform.position, hitGameObject.transform.position));
			float damageToApply = damage;
			damageableObject.ApplyDamage(damageToApply, sourceSpaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}


	IEnumerator Explode() {
		SphereCollider sphereCollider = this.collider as SphereCollider;
		float originalRadius = sphereCollider.radius;
		explosion.SetActive(true);
		explosion.transform.parent = null;
		missileModel.enabled = false;
		
		while (timeUntilMaxExplosionRadius > 0) {
			timeUntilMaxExplosionRadius -= Time.deltaTime;
			sphereCollider.radius = Mathf.Lerp(sphereCollider.radius, maxExplosionRadius, Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		GameObject.Destroy(this.gameObject);
		yield break;
	}
	
 }
