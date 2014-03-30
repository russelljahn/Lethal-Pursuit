using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class AuroraCannon : Pickup {
	
	public float damageRate = 30.0f;
	public float laserHitForce = 1000.0f;
	public float currentEnergy = 100f;
	public float energyDrainRate = 50.0f;
	public float laserLength = 750.0f;
	private LineRenderer line;
	private GameObject hitGameObject;
	

	public override void Start() {
		line = GetComponent<LineRenderer>();
	}


	public override void Update() {
//		Debug.Log ("spaceship: " + spaceship);
		if (spaceship != null && spaceship.shooting) {
			line.enabled = true;
			currentEnergy -= energyDrainRate*Time.deltaTime;
			
			Ray ray = new Ray(spaceship.spaceshipModel.transform.position, spaceship.spaceshipModel.transform.forward);
			RaycastHit hit;

			line.SetPosition(0, Vector3.zero);
			if (Physics.Raycast(ray, out hit)) {
				line.SetPosition(1, this.transform.InverseTransformPoint(hit.point));
				
				/* Apply laser force. */
				if (hit.rigidbody != null) {
					hitGameObject = hit.collider.gameObject;
					hit.rigidbody.AddForceAtPosition(ray.direction*laserHitForce, hit.point);
				}
			}
			else {
				line.SetPosition(1, new Vector3(0.0f, 0.0f, laserLength));
			}
		}
		else {
			hitGameObject = null;
			line.enabled = false;
		}
	}


	void FixedUpdate() {
		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));

		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageRate, hitGameObject, gameObject.name + " is calling ApplyDamage()!");
		}
	}


	public override bool ShouldDrop() {
		return currentEnergy <= 0.0f;
	}

	
}
