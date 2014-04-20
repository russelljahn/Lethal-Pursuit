using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class HexLaserBullet : Bullet {

	public GameObject target;
	public float homingSensitivity = 0.05f;
	public float hitRadius = 30.0f;

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
		}
	}


	private void HandleHit(GameObject hitGameObject) {
		alreadyDying = true;
		explosion.transform.parent = null;
		explosion.SetActive(true);
		HandleApplyingDamage(hitGameObject);
		GameObject.Destroy(this.gameObject);
	}

}
	