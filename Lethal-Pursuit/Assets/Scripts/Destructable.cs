using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour, IDamageable {

	public float health = 100f;

	public void ApplyDamage(float amount) {
		health = Mathf.Max(0, health - amount);

		if (health <= 0.0f) {
			this.gameObject.SetActive(false);
		}
	}
}
