using UnityEngine;
using System.Collections;

public class DisableInWebPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (Application.isWebPlayer) {
			this.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
