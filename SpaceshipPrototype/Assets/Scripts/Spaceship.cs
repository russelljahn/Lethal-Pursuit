using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (MeshCollider))]
public class Spaceship : MonoBehaviour {


	public Vector3 acceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 10000.0f, 10000.0f);

	private Rigidbody rigidbody;
	private ParticleSystem flames;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		flames = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		float zForce;
		if (Input.GetButton("Fire1")) {
			zForce = acceleration.z;
			flames.Play();
		}
		else {
			zForce = 0.0f;
			flames.Stop();
		}

		Vector3 force = new Vector3(
			0.0f, 
			0.0f,
			zForce

		);

		Vector3 torque = new Vector3(
			-xTilt*acceleration.x, 
			-yTilt*acceleration.y, 
			0.0f
		);

		rigidbody.AddRelativeForce(force);
//		rigidbody.AddRelativeTorque(torque);

		Debug.Log("force: " + force);
		Debug.Log("torque: " + torque);
		
		this.transform.Rotate(
			new Vector3(
				-yTilt*acceleration.y, 
				xTilt*acceleration.x,
				0.0f
			)
		);
	}
}
