using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipMesh : SpaceshipComponent, IDamageable {

	IDamageable damageableObject;

	void Start() {
		IDamageable damageableObject = (IDamageable)gameObject.GetComponent(typeof(IDamageable));
	}

	public void ApplyDamage(float amount) {
		if (damageableObject != null) {
			damageableObject.ApplyDamage(amount);
		}
	} 


}
