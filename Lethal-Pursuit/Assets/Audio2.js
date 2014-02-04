#pragma strict

function Start () {

}

var engine4: AudioClip; 
var engine3: AudioClip;
var engine1: AudioClip;

var audioClipSpeed = 10.0; //default audio clip speed

function Update() {
	var p = rigidbody.velocity.magnitude / audioClipSpeed;
	audio.pitch = Mathf.Clamp( p, 0.1, 4.0); // p is clamped to limiting values .1 and 4
	
	
	if(Input.GetButtonDown("Fire1")){
		audio.Play();

	}
	
	
	if (Input.GetButtonUp("Fire1")){
		// you can use PlayOneShot to specify the sound...
		audio.Stop();
		// or you can define the sound in the Inspector
		// and use Play like this:
	}
	
}

