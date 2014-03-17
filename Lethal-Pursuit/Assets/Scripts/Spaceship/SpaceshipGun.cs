using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipGun : SpaceshipComponent {
	

	public AudioSource guns;
	public AudioClip shot;

	private Light light;
	private LineRenderer line;

	public float laserMinWidth = 5f;
	public float laserFullChargeExtraWidth = 5f;
	public float laserWaverRate = 4.0f;

	public float laserMaterialScrollSpeedU = 1.0f;
	public float laserMaterialScrollSpeedV = 1.0f;

	public float damageRate = 1.0f;
	public float laserHitForce = 1000.0f;

	private GameObject hitGameObject;

	public GameObject explosion;
	public float explosionCooldown = 0.1f;
	public float timeSinceLastExplosion;





	// Use this for initialization
	public override void Start () {
		base.Start();

		line = GetComponent<LineRenderer>();
		light = GetComponent<Light>();

		line.enabled = false;
		light.enabled = false;

		explosion.SetActive(false);
	}





	// Update is called once per frame
	public override void Update () {
		base.Update();	

		if (shooting) {
			line.enabled = true;
			light.enabled = true;

			Ray ray = new Ray(this.transform.position, forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit)) {
				line.SetPosition(0, Vector3.zero);
				line.SetPosition(1, this.transform.InverseTransformPoint(hit.point));

				/* Apply laser force. */
				if (hit.rigidbody != null) {
					hitGameObject = hit.collider.gameObject;
					hit.rigidbody.AddForceAtPosition(ray.direction*laserHitForce, hit.point);
				}

				/* Spawn laser explosions. */
				if (timeSinceLastExplosion >= explosionCooldown) {
					GameObject newExplosion = GameObject.Instantiate(explosion, hit.point, Quaternion.identity) as GameObject;
					newExplosion.SetActive(true);
					timeSinceLastExplosion = 0.0f;
				}
			}
			line.SetWidth(laserMinWidth+laserFullChargeExtraWidth*Mathf.Cos(laserWaverRate*Time.time), laserMinWidth+laserFullChargeExtraWidth*Mathf.Sin(laserWaverRate*Time.time));
			renderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time*laserMaterialScrollSpeedU, Time.time*laserMaterialScrollSpeedV));
		}
		else {
			hitGameObject = null;
			line.enabled = false;
			light.enabled = false;
		}

		timeSinceLastExplosion += Time.deltaTime;

	}





	void FixedUpdate() {

		if (hitGameObject == null) {
			return;
		}
		IDamageable damageableObject = (IDamageable)hitGameObject.GetComponent(typeof(IDamageable));

//		Debug.Log ("hitGameObject: " + hitGameObject.name);
//		Debug.Log ("hitGameObject is IDamageable: " + (damageableObject is IDamageable));

		if (damageableObject != null) {
			damageableObject.ApplyDamage(damageRate);
		}
	}




}
