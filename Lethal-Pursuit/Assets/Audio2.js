#pragma strict

import UnityEngine;
import InControl;
import System.Collections;

function Start () {

}

var engine4: AudioClip; 
var engine3: AudioClip;
var engine1: AudioClip;
var audioClipSpeed= .05; //default audio clip speed

function Update() {
	var r : float;
	var p = rigidbody.velocity.magnitude / audioClipSpeed + r;
	var v = audio.volume;
	var boostAmount :float = InputManager.ActiveDevice.RightTrigger.Value;
	
	audio.pitch = Mathf.Clamp( p, (.8 + (Random.Range(-.25, .25))), 6 + ((Random.Range(-.25, .25)))); // p is clamped to limiting values .1 and 4
	audio.volume = (Random.Range(.8, 1)); // p is clamped to limiting values .1 and 4
	
	Debug.Log(p);
	
	
	if(Input.GetButtonDown("Fire1")){
		audio.Play();
		

	}
	
	
	if (Input.GetButtonUp("Fire1")){
		// you can use PlayOneShot to specify the sound...
		audio.Stop();
		r = Random.value;
		Mathf.Clamp( r,10,-10);
		Debug.Log(r);
	
		// or you can define the sound in the Inspector
		// and use Play like this:
	}
	
}

