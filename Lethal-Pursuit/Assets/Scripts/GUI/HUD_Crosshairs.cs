using UnityEngine;
using System.Collections;

public class HUD_Crosshairs : MonoBehaviour {


	public Spaceship spaceship;
	public float movementSpeed = 1.0f;
	public float realignSpeed = 2.0f;

	public float xExtent = 100.0f;
	public float yExtent = 50.0f;

	private Vector3 initialLocalPosition;


	void Start() {
		initialLocalPosition = this.transform.localPosition;
	}
	

	void Update () {

		if (spaceship.xTilt != 0.0f || spaceship.yTilt != 0.0f) {
			this.transform.localPosition = Vector3.Slerp (
				this.transform.localPosition, 
				initialLocalPosition + new Vector3(xExtent*spaceship.xTilt, yExtent*spaceship.yTilt, 0.0f), 
				movementSpeed*Time.deltaTime
			);
		}
		else {
			this.transform.localPosition = Vector3.Slerp (
				this.transform.localPosition, 
				initialLocalPosition,
				realignSpeed*Time.deltaTime
			);
		}

	}



}
