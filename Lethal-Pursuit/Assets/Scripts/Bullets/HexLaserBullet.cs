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
			Vector3 thisToTarget = target.transform.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(thisToTarget);
			transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, homingSensitivity);
		}
		this.rigidbody.MovePosition(this.transform.position + speed*this.transform.forward*Time.deltaTime);
	}



	public override void OnTriggerStay(Collider other) {

		GameObject hitGameObject = other.gameObject;

		if (target == null && 
		    hitGameObject != sourceSpaceship.gameObject && 
		    Vector3.Distance(hitGameObject.transform.position, this.transform.position) > hitRadius) {

			ITargetable targetableObject = (ITargetable)hitGameObject.GetComponent(typeof(ITargetable));
			if (targetableObject != null) {
				target = hitGameObject;
			}
			SphereCollider sphereCollider = this.collider as SphereCollider;
			sphereCollider.radius = hitRadius;
		}
		else if (!alreadyDying && ShouldExplodeOnContact(other.gameObject)) {
//			this.transform.position = other.transform.position;
			alreadyDying = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			HandleApplyingDamage(other.gameObject);
			GameObject.Destroy(this.gameObject);
		}
	}



	public override void HandleApplyingDamage(GameObject hitGameObject) {
		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));
		
		//		Debug.Log ("hitGameObject: " + hitGameObject.name);
		//		Debug.Log ("hitGameObject is IDamageable: " + (damageableObject is IDamageable));
		
		if (damageableObject != null) {
			damageableObject.ApplyDamage(damage, sourceSpaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}

}
