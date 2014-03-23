using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipMesh : SpaceshipComponent {

	IDamageable damageableObject;

	public override void Start() {
		base.Start();
		damageableObject = (IDamageable)gameObject.GetComponent(typeof(IDamageable));
	}

	public void ApplyDamage(float amount) {
		if (damageableObject != null) {
			//damageableObject.ApplyDamage(amount);
		}
	} 
}
