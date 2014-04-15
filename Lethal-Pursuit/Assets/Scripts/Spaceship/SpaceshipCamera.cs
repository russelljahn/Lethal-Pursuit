using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipCamera : MonoBehaviour {

	public Spaceship spaceship;
	public GameObject target;
	
	public float rotationSpeed = 3.0f;
	public float lookSpeed = 0.7f;
	private Vector3 cameraToModel;
	
	
	
	public void SetSpaceship(Spaceship spaceship) {
		this.spaceship = spaceship;
		this.transform.parent = spaceship.transform;
	}
	
	
	
	void Start() {
		cameraToModel = this.transform.localPosition;
	}
	
	
	
	void Update() {
		
		Vector3 newCameraPosition = spaceship.spaceshipModelRoll.transform.TransformPoint(
			spaceship.spaceshipModelRoll.transform.localPosition + cameraToModel
			);
				
		this.transform.position = Vector3.Lerp(
			this.transform.position, 
			newCameraPosition, 
			Mathf.Clamp01(rotationSpeed*Time.deltaTime)
			);
		
		Vector3 lookPoint = target.transform.position;
		
		Quaternion targetRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Clamp01(lookSpeed*Time.deltaTime));
		
		
	}
	
	
	
}