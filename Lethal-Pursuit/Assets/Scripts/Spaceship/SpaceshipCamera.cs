using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipCamera : MonoBehaviour {


	public Spaceship spaceship;
	public GameObject pointToLookAt;

	public float yLookAmount = 0.8f;
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

		Vector3 newCameraPosition = spaceship.spaceshipModel.transform.TransformPoint(
			spaceship.spaceshipModel.transform.localPosition + cameraToModel
		);

//		newCameraPosition.y = Mathf.Abs(newCameraPosition.y);

		this.transform.position = Vector3.Lerp(
			this.transform.position, 
			newCameraPosition, 
			Mathf.Clamp01(rotationSpeed*Time.deltaTime)
		);

		Vector3 lookPoint = pointToLookAt.transform.position;

		Quaternion targetRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Clamp01(lookSpeed*Time.deltaTime));


	}



}
