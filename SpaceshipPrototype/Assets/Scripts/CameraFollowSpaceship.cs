using UnityEngine;
using System.Collections;

public class CameraFollowSpaceship : MonoBehaviour {


	public Spaceship spaceship;
	private Vector3 distanceSpaceshipToCamera;

	void Start () {
		distanceSpaceshipToCamera = this.transform.position - spaceship.transform.position;
	}
	
	void FixedUpdate () {
		this.transform.position = spaceship.transform.position + distanceSpaceshipToCamera;
	}
}
