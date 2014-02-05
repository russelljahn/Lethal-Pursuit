using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipAudio : MonoBehaviour {

	public AudioClip engine4;
	public AudioClip engine1; 
	public AudioClip engine3; 
	float pitch; 
	float volume;
	private float speed;
	private float boostTime;
	private float brakeTime;
	float randomizer = Random.value;
	float audioClipSpeed = .5f; //default speed


	private float maxBoostTime;


	void Start () {

		pitch = rigidbody.velocity.magnitude / audioClipSpeed;
		float volume = audio.volume;
		Mathf.Clamp(randomizer ,1,-1);
		audio.pitch = 0.0f;
	}

	void Update () {

		float boostAmount = InputManager.ActiveDevice.RightTrigger.Value;
		float brakeAmount = InputManager.ActiveDevice.LeftTrigger.Value;
		
		Random.seed = 1;
		//audio.pitch = Random.Range(.25f,.5f); //creates turbulence
//		Debug.Log(volume);
						
		if(boostAmount > 0){
			audio.volume = boostAmount + (Random.Range(-0.25f,-.01f));

				audioClipSpeed += Random.value;
				boostTime += Time.deltaTime;
				audio.pitch = Mathf.Min(3, boostTime+Time.deltaTime);
				Random.seed++;

			}
				
		else if (boostAmount == 0){
			// you can use PlayOneShot to specify the sound...
				audio.volume = 0.15f + (Random.Range(-0.05f,.25f ));
				boostTime = Mathf.Max(0.0f, boostTime-Time.deltaTime, 2);
				boostTime -= 1;
						 
			}
		if(brakeAmount > 0){
		//	speed = 0.001f*(transform.position.z)/ audioClipSpeed;
			brakeTime += Time.deltaTime;
			boostTime = Mathf.Max(1.3f, boostTime-brakeTime);
			audio.volume = (1-.5f * brakeAmount) + (Random.Range(-0.01f,.01f ));
		
		}

				
		audio.pitch = boostTime / maxBoostTime;

		//audio.pitch = speed;
	}

}