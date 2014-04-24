using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class StickyMine : MonoBehaviour {

	public Spaceship sourceSpaceship;
	public GameObject explosion;

	public float damage = 200f;
	public float timeUntilDeath = 5.0f;
	protected bool alreadyDying = false;

	public MeshRenderer mineModel;

	public float stuckRadius = 2.0f;
	public float maxExplosionRadius = 100.0f;
	public float timeUntilMaxExplosionRadius = 0.05f;
	private GameObject stuckObject;

	private Vector3 stuckObjectToMine;
	public AudioClip explosionSound;


	public void Start() {
		Random.seed = (int)Time.time;
		this.transform.rotation = Random.rotation;
	}


	public void FixedUpdate () {
		timeUntilDeath -= Time.deltaTime;

		if (stuckObject != null) {
			this.transform.position = stuckObject.transform.position + stuckObjectToMine;
		}

		if (timeUntilDeath <= 0 && !alreadyDying) {
			alreadyDying = true;
			StartCoroutine(Explode());
		}
	}


	IEnumerator Explode() {
//		explosion.audio.PlayOneShot(explosionSound);
		SphereCollider sphereCollider = this.collider as SphereCollider;
		float originalRadius = sphereCollider.radius;
		explosion.SetActive(true);
		explosion.transform.parent = null;
		mineModel.enabled = false;	

		// Damage spawner if within vicinity
		if (Vector3.Distance(sourceSpaceship.transform.position, this.transform.position) <= maxExplosionRadius) {
			HandleApplyingDamage(sourceSpaceship.gameObject);
		}

		if (stuckObject != null) {
			HandleApplyingDamage(stuckObject);
		}

		while (timeUntilMaxExplosionRadius > 0) {
			timeUntilMaxExplosionRadius -= Time.deltaTime;
			sphereCollider.radius = Mathf.Lerp(sphereCollider.radius, maxExplosionRadius, Time.deltaTime);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		if (this.gameObject != null) {
			GameObject.Destroy(this.gameObject);
		}
		yield break;
	}


	void OnCollisionEnter(Collision collision) {
		// Damage object if exploding
		if (alreadyDying) {
			HandleApplyingDamage(collision.gameObject);
			return;
		}

		// Ignore sticking to spawner ship when it first ejects mine
		if (stuckObject == null && sourceSpaceship != null && collision.gameObject == sourceSpaceship.gameObject) {
			return;
		}
		// Explode if shot
		else if (!alreadyDying && collision.gameObject.CompareTag("Bullet")) {
			alreadyDying = true;
			StartCoroutine(Explode());
			return;
		}

		// Stick on whatever collides
		if (!alreadyDying) {
			SphereCollider sphereCollider = this.collider as SphereCollider;
			sphereCollider.radius = stuckRadius;
			stuckObjectToMine = this.transform.position - collision.transform.position;
			stuckObject = collision.gameObject;

			//Play Sound
		}
		else {
			HandleApplyingDamage(collision.gameObject);
		}
	}


	void HandleApplyingDamage(GameObject hitGameObject) {
		if (hitGameObject == null || sourceSpaceship.gameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));

		if (damageableObject != null) {
			// Eventually should have some damage falloff.
//			float damageToApply = 1.0f/(1.0f+Vector3.Distance(this.gameObject.transform.position, hitGameObject.transform.position));
			float damageToApply = damage;
			if (Network.isClient) {
				damageToApply = 0f;
			}
			damageableObject.ApplyDamage(damageToApply, sourceSpaceship.gameObject, gameObject.name + " is calling ApplyDamage()!");
			
		}
	}



	
 }
