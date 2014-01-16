using UnityEngine;
using System.Collections;

public class CameraFollowSpaceship : MonoBehaviour {


	public Spaceship spaceship;
	public Vector3 distanceSpaceshipToCamera = new Vector3(0.0f, 150.0f, -350.0f);
	

	void FixedUpdate () {
		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;
	}
}
