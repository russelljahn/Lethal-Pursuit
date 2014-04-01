using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour, IDamageable {
	
	public float health = 100f;
	public float damageRate = 0.5f;
	
	
	// Implementing IDamageable interface.
	public void ApplyDamage(float amount, GameObject damager, string message) {
		health = Mathf.Max(0, health - amount);
		
		if (IsDead()) {
			this.gameObject.SetActive(false);
		}
	}
	public bool IsDead() {
		return health <= 0.0f;
	}
	
	
	void OnCollisionStay(Collision collision) {
		
		if (collision.gameObject.CompareTag("Obstacle")) {
			return;
		}
		
		IDamageable damageableObject = (IDamageable)collision.gameObject.GetComponent(typeof(IDamageable));
		
		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageRate, gameObject, "Calling from: " + gameObject.name);
		}
	}
	
	
	
	
}