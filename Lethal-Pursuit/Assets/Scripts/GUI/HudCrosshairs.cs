using UnityEngine;
using System.Collections;

public class HudCrosshairs : SpaceshipComponent {

	
	public SpaceshipCamera spaceshipCamera;


	public float movementSpeed = 100.0f;
	public float xExtent = 10000f;
	public float yExtent = 10000f;
	

	private Vector3 initialLocalPosition;

	public bool debug;
	private LineRenderer debugLine;

	void Start() {
		initialLocalPosition = this.transform.localPosition;

		debugLine = gameObject.GetComponent<LineRenderer>();
		debugLine.SetWidth(10f, 10f);
		debugLine.SetColors(Color.blue, Color.green);

<<<<<<< HEAD
		this.renderer.material.renderQueue = 4000;
//		this.transform.parent = null;
=======
>>>>>>> 5d2353ada24dfbfbad2d973c89b8f0975f0e56af
	}
	

	void Update () {

<<<<<<< HEAD
//		this.transform.position = spaceship.transform.position - initialLocalPosition;

=======
>>>>>>> 5d2353ada24dfbfbad2d973c89b8f0975f0e56af
		if (debug) {
			debugLine.enabled = true;
			debugLine.SetPosition(0, spaceship.spaceshipModel.transform.position);
			debugLine.SetPosition(1, this.transform.position);
		}
		else {
			debugLine.enabled = false;
		}
//
//		this.transform.RotateAround(spaceship.transform.position, Vector3.up, Time.deltaTime*movementSpeed*xTiltRight);
//		this.transform.RotateAround(spaceship.transform.position, Vector3.left, Time.deltaTime*movementSpeed*yTiltRight);
//		

//		if (spaceship.xTiltRight != 0.0f || spaceship.yTiltRight != 0.0f) {
//			this.transform.localPosition = Vector3.Slerp (
//				this.transform.localPosition, 
//				initialLocalPosition + new Vector3(0.0f, -spaceship.xTiltRight*xExtent, spaceship.yTiltRight*yExtent), 
//				movementSpeed*Time.deltaTime
//			);
//		}



//		else {
//			this.transform.localPosition = Vector3.Slerp (
//				this.transform.localPosition, 
//				initialLocalPosition,
//				realignSpeed*Time.deltaTime
//			);
//		}

		// Make crosshairs GUI plane face the viewport straight-on
//		this.transform.rotation = spaceshipCamera.transform.rotation  * Quaternion.Euler(90, 180, 0);
	}

}
