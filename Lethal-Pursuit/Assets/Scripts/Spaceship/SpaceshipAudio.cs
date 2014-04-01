using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipAudio : SpaceshipComponent {

	public AudioSource engine1;
	public AudioSource engine2;
	public AudioSource engine3;
	public AudioSource drift1;

	public AudioClip engine1Clip;
	public AudioClip engine2Clip;
	public AudioClip engine3Clip;
	public AudioClip drift1Clip;

	public float engine1MinVolume = 0.0f;
	public float engine1MaxVolume = 1.0f;
	public float engine2MinVolume = 0.0f;
	public float engine2MaxVolume = 0.7f;
	public float engine3MinVolume = 0.0f;
	public float engine3MaxVolume = 1.0f;
	public float drift1MinVolume = 0.0f;
	public float drift1MaxVolume = 1.0f;

	public float engine1VolumeFadeRate = 1.0f;
	public float engine2VolumeFadeRate = 1.0f;
	public float engine3VolumeFadeRate = 1.0f;
	public float drift1VolumeFadeRate = 1.0f;

	public float engine1MinPitch = 1.0f;
	public float engine1MaxPitch = 1.0f;
	public float engine2MinPitch = 1.0f;
	public float engine2MaxPitch = 1.0f;
	public float engine3MinPitch = 1.0f;
	public float engine3MaxPitch = 2.0f;
	public float drift1MinPitch = 1.0f;
	public float drift1MaxPitch = 1.0f;

	public float engine1PitchFadeRate = 1.0f;
	public float engine2PitchFadeRate = 1.0f;
	public float engine3PitchFadeRate = 1.0f;
	public float drift1PitchFadeRate = 1.0f;



	public override void Start () {
		base.Start();
		engine1.clip = engine1Clip;
		engine1.loop = true;
		engine1.Play();

		engine2.clip = engine2Clip;
		engine2.loop = true;
		engine2.Play();;

		engine3.clip = engine3Clip;
		engine3.loop = true;
		engine3.Play();

		drift1.clip = drift1Clip;
		drift1.loop = true;
		drift1.Play();
	}



	public override void Update () {

		//engine1.volume = Mathf.Lerp(engine1.volume, engine1MinVolume+(engine1MaxVolume-engine1MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine1VolumeFadeRate);
		engine2.volume = Mathf.Lerp(engine2.volume, engine2MinVolume+(engine2MaxVolume-engine2MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine2VolumeFadeRate);
		engine2.pitch = Mathf.Lerp(engine2.pitch, engine3MinVolume+(engine1MaxVolume-engine3MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine3VolumeFadeRate);
		engine3.volume = Mathf.Lerp(engine3.volume, engine3MinVolume+(engine1MaxVolume-engine3MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine3VolumeFadeRate);
		engine3.pitch = Mathf.Lerp(engine3.pitch, engine3MinVolume+(engine1MaxVolume-engine3MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine3VolumeFadeRate);

		//	drift1.volume = Mathf.Lerp(drift1.volume, drift1MinVolume+(drift1MaxVolume-drift1MinVolume)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*drift1VolumeFadeRate);
		

		//engine1.pitch = Mathf.Lerp(engine1.pitch, engine1MinPitch+(engine1MaxPitch-engine1MinPitch)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine1PitchFadeRate);
		//engine2.pitch = Mathf.Lerp(engine2.pitch, engine2MinPitch+(engine1MaxPitch-engine2MinPitch)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine2PitchFadeRate);
		//engine3.pitch = Mathf.Lerp(engine3.pitch, engine3MinPitch+(engine1MaxPitch-engine3MinPitch)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*engine3PitchFadeRate);
		//drift1.pitch = Mathf.Lerp(drift1.pitch, drift1MinPitch+(drift1MaxPitch-drift1MinPitch)*spaceship.currentBoostVelocity/spaceship.maxBoostVelocity, Time.deltaTime*drift1PitchFadeRate);
		
	}



}