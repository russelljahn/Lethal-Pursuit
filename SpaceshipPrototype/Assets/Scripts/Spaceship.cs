using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {


	public Vector3 acceleration = new Vector3(200.0f, 150.0f, 200.0f);
	public Vector3 maxVelocity  = new Vector3(0.0f, 10000.0f, 10000.0f);

	private Rigidbody rigidbody;
	private ParticleSystem flames;
	public GameObject pivot;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		flames = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
	}
	

	void FixedUpdate () {
	
		float xTilt = Input.GetAxis("Horizontal");
		float yTilt = Input.GetAxis("Vertical");

		float zForce;

		rigidbody.velocity = new Vector3(
			rigidbody.velocity.x,
			rigidbody.velocity.y,
			Mathf.Max(0.0f, rigidbody.velocity.z)
		);

		if (Input.GetButton("Boost")) { //Spacebar by default will make it move forward
			rigidbody.AddRelativeForce (Vector3.forward*acceleration.z*Time.deltaTime);
			flames.Play();
		}
		else {
			flames.Stop();
		}

		this.transform.Translate(Input.GetAxis("Horizontal")*Time.deltaTime*acceleration.x, 0.0f, 0.0f);
		this.transform.Translate(0.0f, -Input.GetAxis("Vertical")*Time.deltaTime*acceleration.y, 0.0f);

//		Debug.Log ("transform.rotation.eulerAngles: " + transform.rotation.eulerAngles);

			transform.RotateAround(
				pivot.transform.position, 
				Vector3.forward, 
				Time.deltaTime*Input.GetAxis("Horizontal")*acceleration.x*0.1f
			);
//		}
	

//		if (Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Vertical") == 0.0f) {
//			Vector3 newDirection = Vector3.RotateTowards(
//				this.transform.position, 
//				Vector3.zero, 
//				Mathf.PI/60.0f, 
//				10.0f
//			);
//			transform.localRotation = Quaternion.LookRotation(newDirection);
//		}
	
		

	}
}
