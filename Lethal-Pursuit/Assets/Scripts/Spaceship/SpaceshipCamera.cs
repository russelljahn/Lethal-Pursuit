using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipCamera : MonoBehaviour {


	public Spaceship spaceship;
	public HUD_Crosshairs crosshairs;

	public float rotationSpeed = 1.0f;
	public Vector3 cameraToModel;


	public void SetSpaceship(Spaceship spaceship) {
		this.spaceship = spaceship;
		this.transform.parent = spaceship.transform;
	}



	void Start() {
		cameraToModel = this.transform.localPosition;
	}

	


	void FixedUpdate() {

		Vector3 newCameraPosition = spaceship.spaceshipModel.transform.TransformPoint(
			spaceship.spaceshipModel.transform.localPosition + cameraToModel
		);

		this.transform.position = Vector3.Lerp(
			this.transform.position, 
			newCameraPosition, 
			rotationSpeed*Time.deltaTime*Vector3.Distance(spaceship.spaceshipModel.transform.forward, this.transform.forward)
		);

		this.transform.LookAt(crosshairs.transform.position);

	}
}
