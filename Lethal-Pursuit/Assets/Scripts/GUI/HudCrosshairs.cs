using UnityEngine;
using System.Collections;

public class HudCrosshairs : SpaceshipComponent {

	
	public SpaceshipCamera spaceshipCamera;
	
	public float movementSpeed = 100.0f;
	public float xExtent = 10000f;
	public float yExtent = 10000f;

	public bool debug;
	private LineRenderer debugLine;


	public override void Start() {

		debugLine = gameObject.GetComponent<LineRenderer>();
		debugLine.SetWidth(10f, 10f);
		debugLine.SetColors(Color.blue, Color.green);

		this.renderer.material.renderQueue = 4000;
	}


	public override void Update () {
		if (debug) {
			debugLine.enabled = true;
			debugLine.SetPosition(0, spaceshipModel.transform.position);
			debugLine.SetPosition(1, this.transform.position);
		}
		else {
			debugLine.enabled = false;
		}

		// Make crosshairs GUI plane face the viewport straight-on
//		this.transform.rotation = spaceshipCamera.transform.rotation  * Quaternion.Euler(90, 180, 0);
	}

}
