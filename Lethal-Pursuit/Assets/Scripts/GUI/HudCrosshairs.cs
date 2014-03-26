using UnityEngine;
using System.Collections;

public class HudCrosshairs : MonoBehaviour {


	public Spaceship spaceship;
	public SpaceshipCamera spaceshipCamera;

	public float movementSpeed = 1.0f;
	public float realignSpeed = 2.0f;

	public float xExtent = 100.0f;
	public float yExtent = 50.0f;

	private Vector3 initialLocalPosition;

	public bool debug;
	private LineRenderer debugLine;

	void Start() {
		initialLocalPosition = this.transform.localPosition;

		debugLine = gameObject.GetComponent<LineRenderer>();
		debugLine.SetWidth(10f, 10f);
		debugLine.SetColors(Color.blue, Color.green);

	}
	

	void Update () {

		return;

		if (debug) {
			debugLine.enabled = true;
			debugLine.SetPosition(0, spaceship.spaceshipModel.transform.position);
			debugLine.SetPosition(1, this.transform.position);
		}
		else {
			debugLine.enabled = false;
		}

		if (spaceship.xTilt != 0.0f || spaceship.yTilt != 0.0f) {
			this.transform.localPosition = Vector3.Slerp (
				this.transform.localPosition, 
				initialLocalPosition + new Vector3(xExtent*spaceship.xTilt, yExtent*spaceship.yTilt, 0.0f), 
				movementSpeed*Time.deltaTime
			);
		}
//		else {
//			this.transform.localPosition = Vector3.Slerp (
//				this.transform.localPosition, 
//				initialLocalPosition,
//				realignSpeed*Time.deltaTime
//			);
//		}

		// Make crosshairs GUI plane face the viewport straight-on
		this.transform.rotation = spaceshipCamera.transform.rotation  * Quaternion.Euler(90, 180, 0);
	}

}
