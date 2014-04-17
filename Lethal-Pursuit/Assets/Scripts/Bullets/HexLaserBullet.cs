using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HexLaserBullet : Bullet {

	

	public override void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;
		
		if (timeUntilDeath <= 0 && !alreadyDying) {
			alreadyDying = true;
			GameObject.Destroy(this.gameObject);
		}
		this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
	}



	public override void OnCollisionEnter(Collision collision) {
		if (!alreadyDying && ShouldExplodeOnContact(collision.gameObject)) {
			this.transform.position = collision.contacts[0].point;
			alreadyDying = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			HandleApplyingDamage(collision.gameObject);
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
