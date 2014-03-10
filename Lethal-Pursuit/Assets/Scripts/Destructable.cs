using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour, IDamageable {

	public float health = 100f;
	public float damageRate = 0.5f;


	public void ApplyDamage(float amount) {
		health = Mathf.Max(0, health - amount);

		if (health <= 0.0f) {
			this.gameObject.SetActive(false);
		}
	}


	void OnCollisionStay(Collision collision) {

		if (collision.gameObject.CompareTag("Obstacle")) {
			return;
		}

		IDamageable damageableObject = (IDamageable)collision.gameObject.GetComponent(typeof(IDamageable));

		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageRate);
		}
	}

}
