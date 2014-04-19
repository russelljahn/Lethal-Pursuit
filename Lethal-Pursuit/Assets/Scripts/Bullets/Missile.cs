using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class Missile : Bullet {

	public MeshRenderer missileModel;
	
	public GameObject target;
	public float homingSensitivity = 0.05f;
	public float hitRadius = 30.0f;
	public float maxExplosionRadius = 300.0f;
	public float timeUntilMaxExplosionRadius = 0.05f;


	public override void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;
		
		if (timeUntilDeath <= 0 && !alreadyDying) {
			alreadyDying = true;
			GameObject.Destroy(this.gameObject);
		}
		if (target != null) {
			if (Vector3.Distance(this.transform.position, target.transform.position) <= 6.0f*hitRadius) {
				this.HandleHit(target);
				return;
			}
			
			Vector3 thisToTarget = target.transform.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(thisToTarget);
			transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, homingSensitivity);
		}
		if (!alreadyDying) {
			this.rigidbody.MovePosition(this.transform.position + speed*this.transform.forward*Time.deltaTime);
		}
	}
	
	
	public void OnTriggerEnter(Collider other) {
		this.OnTriggerStay(other);
	}
	
	
	public override void OnTriggerStay(Collider other) {
		
		GameObject hitGameObject = other.gameObject;
		SphereCollider sphereCollider = this.collider as SphereCollider;
		
		if (target == null && 
		    hitGameObject != sourceSpaceship.gameObject && 
		    Vector3.Distance(hitGameObject.transform.position, this.transform.position) > hitRadius
		    ) {
			
			ITargetable targetableObject = (ITargetable)hitGameObject.GetComponent(typeof(ITargetable));
			if (targetableObject != null) {
				target = hitGameObject;
				sphereCollider.radius = hitRadius;
				return;
			}
		}
		if (!alreadyDying && ShouldExplodeOnContact(other.gameObject)) {
			if (hitGameObject == target || Vector3.Distance(hitGameObject.transform.position, this.transform.position) <= hitRadius) {
				HandleHit(other.gameObject);
			}
			//			this.transform.position = other.transform.position;
			
		}
	}
	
	
	private void HandleHit(GameObject hitGameObject) {
		alreadyDying = true;
		explosion.transform.parent = null;
		explosion.SetActive(true);
		HandleApplyingDamage(hitGameObject);
		GameObject.Destroy(this.gameObject);
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


//	void FixedUpdate () {
//		timeUntilDeath -= Time.deltaTime;
//		if (!alreadyDying && timeUntilDeath <= 0) {
//			alreadyDying = true;
//			StartCoroutine(Explode());
//		}
//
//		if (!alreadyDying) {
//			this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
//		}
//	}

//
//	void OnCollisionEnter(Collision collision) {
//		// Ignore explosion if spaceship just shot missile and is within the collider
//		if (!alreadyDying && sourceSpaceship != null && collision.gameObject == sourceSpaceship.gameObject) {
//			return;
//		}
//		if (!alreadyDying && ShouldExplodeOnContact(collision.gameObject)) {
//			this.transform.position = collision.contacts[0].point;
//			alreadyDying = true;
//			StartCoroutine(Explode());
//		}
//		HandleApplyingDamage(collision.gameObject);
//	}
//
//
//	bool ShouldExplodeOnContact(GameObject other) {
////		Debug.Log ("other == sourceSpaceship.gameObject? " + (other == sourceSpaceship.gameObject));
//		if (other.CompareTag("Bullet")) {
//			return false;
//		}
//		return sourceSpaceship != null && other != sourceSpaceship.gameObject;
//	}
//
//
//	void HandleApplyingDamage(GameObject hitGameObject) {
//		if (hitGameObject == null) {
//			return;
//		}
//		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));
//
//		if (damageableObject != null) {
//			// Eventually should have some damage falloff.
////			float damageToApply = 1.0f/(1.0f+Vector3.Distance(this.gameObject.transform.position, hitGameObject.transform.position));
//			float damageToApply = damage;
//			damageableObject.ApplyDamage(damageToApply, sourceSpaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
//		}
//	}



	
 }
