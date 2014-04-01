using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraFadeTest : MonoBehaviour {

//	public Texture2D tex;

	// Use this for initialization
	void Awake () {
		Debug.Log ("start!");
		Hashtable iTweenSettings = new Hashtable();
		iTweenSettings["oncomplete"] = "FadeOut";
		iTweenSettings["oncompletetarget"] = this.gameObject;
		
		iTweenSettings["amount"] = 0.175f;
		iTweenSettings["time"] = 0.5f;
		iTweenSettings["easetype"] = "easeInOutQuad";

		iTween.CameraFadeAdd(iTween.CameraTexture(Color.red)); // Replace w/ tex eventually
		iTween.CameraFadeTo(iTweenSettings);
	}

	public void FadeOut() {
		Debug.Log ("Fading out");
		Hashtable iTweenSettings = new Hashtable();
		iTweenSettings["amount"] = 0.0f;	
		iTweenSettings["time"] = 0.5f;
		iTweenSettings["easetype"] = "easeInOutQuad";
			
		iTween.CameraFadeFrom(iTweenSettings);
	}
}
