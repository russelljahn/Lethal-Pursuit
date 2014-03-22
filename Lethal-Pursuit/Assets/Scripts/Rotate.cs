using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 rotationRate = Vector3.zero;


	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		this.transform.localRotation = Quaternion.Euler(this.transform.localRotation.eulerAngles + rotationRate*Time.deltaTime);
	}


}
