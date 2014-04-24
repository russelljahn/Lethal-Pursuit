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
	protected bool alreadyDying = false;
	

	public virtual void Start() {
		this.transform.forward = direction;
		collider.isTrigger = true;
	}

	
	public virtual void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;
		if (timeUntilDeath <= 0 && !alreadyDying) {
			alreadyDying = true;
			GameObject.Destroy(this.gameObject);
		}
		this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
	}


	public virtual void OnTriggerStay(Collider collider) {
		if (!alreadyDying && ShouldExplodeOnContact(collider.gameObject)) {
			this.transform.position = collider.transform.position;
			alreadyDying = true;
			explosion.transform.parent = null;
			explosion.SetActive(true);
			HandleApplyingDamage(collider.gameObject);
			GameObject.Destroy(this.gameObject);
		}
	}
	

	public virtual bool ShouldExplodeOnContact(GameObject other) {
		return !other.CompareTag("Bullet") && other != sourceSpaceship.gameObject;
	}


	public virtual void HandleApplyingDamage(GameObject hitGameObject) {
		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));
		
		//		Debug.Log ("hitGameObject: " + hitGameObject.name);
		//		Debug.Log ("hitGameObject is IDamageable: " + (damageableObject is IDamageable));
		float damageToApply = damage;
		if (Network.isClient) {
			damageToApply = 0f;
		}
		
		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageToApply, sourceSpaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}

}
