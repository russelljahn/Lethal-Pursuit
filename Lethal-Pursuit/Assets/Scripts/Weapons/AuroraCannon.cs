using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class AuroraCannon : Pickup {

	public GameObject laserBeamEffect;
	public float damageRate = 30.0f;
	public float laserHitForce = 1000.0f;
	public float currentEnergy = 100f;
	public float energyDrainRate = 50.0f;
	public float laserLength = 500.0f;
	private LineRenderer line;
	private GameObject hitGameObject;
	

	public override void Start() {
		line = GetComponent<LineRenderer>();
	}


	public override void Update() {
//		Debug.Log ("spaceship: " + spaceship);
		if (spaceship != null && spaceship.shooting) {
			line.enabled = true;
			laserBeamEffect.SetActive(true);
			currentEnergy -= energyDrainRate*Time.deltaTime;
			
			Ray ray = new Ray(spaceship.spaceshipModel.transform.position, spaceship.spaceshipModel.transform.forward);
			RaycastHit hit;

			line.SetPosition(0, spaceship.spaceshipModel.transform.position);
			if (Physics.Raycast(ray, out hit)) {
				Debug.Log("ray hit: " + hit.collider.gameObject);
				line.SetPosition(1, hit.point);
				
				/* Apply laser force. */
				if (hit.rigidbody != null) {
					hitGameObject = hit.collider.gameObject;
					hit.rigidbody.AddForceAtPosition(ray.direction*laserHitForce, hit.point);
				}
			}
			else {
				line.SetPosition(1, this.transform.InverseTransformPoint(new Vector3(0.0f, 0.0f, laserLength)));
			}
		}
		else {
			hitGameObject = null;
			line.enabled = false;
			laserBeamEffect.SetActive(false);
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


	public override void OnPickup (Spaceship spaceship) {
		base.OnPickup (spaceship);
		spaceship.DisableGun();
	}


	public override void OnDrop() {
		spaceship.EnableGun();
	}
	
}
