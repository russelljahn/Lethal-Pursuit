using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipAudio : SpaceshipComponent {

	public AudioSource track1;
	public AudioSource track2;
	public AudioSource track3;
	public AudioSource driftSFX;


	public AudioClip wind1;
	public AudioClip engine1; 
	public AudioClip engine3; 
	public AudioClip windStorm;
	public AudioClip driftWind;
	
	private float boostTime = 0.0f;

	private float maxBoostTime = 2.0f;
	
	public float maxPitchEngine1 = 5.0f;

	public float sputteringTurbulenceAmount1 = 1.5f;
	public float sputteringTurbulenceAmount2 = 1.5f;
	public float sputteringTurbulenceRate = 3.0f;



	public override void Start () {
		base.Start();
		track1.clip = engine3;
		track1.loop = true;
		track1.Play();

		track2.clip = wind1;
		track2.loop = true;
		track2.Play();

		track3.clip = windStorm;
		track3.loop = true;
		track3.Play();

		driftSFX.clip = driftWind;


	}



	public override void Update () {
		base.Update();
						
		if (boostAmount > 0) {
			boostTime = Mathf.Min(maxBoostTime, boostTime+Time.deltaTime);
		}
				
		else {
			boostTime = Mathf.Max(0f, boostTime-Time.deltaTime);	
		}

				
		track1.pitch = spaceship.currentVelocity/150 + .3f;
		track1.volume = spaceship.currentVelocity / 150 + .44f;
		track2.pitch = Mathf.Clamp(spaceship.currentVelocity, spaceship.currentVelocity/250 + .4f, 1.1f);
		track2.volume = spaceship.currentVelocity/450+.4f;
		track3.pitch = spaceship.currentVelocity/450 + .4f;
		track3.volume = spaceship.currentVelocity/300- 1;


		track1.pan = xTilt;
		track2.pan = xTilt;


		
		if (heightAboveGround > fractionOfHeightLimitToBeginSputtering*heightLimit) {
			track1.pitch += sputteringTurbulenceAmount2*Mathf.Sin(
				sputteringTurbulenceAmount1*Mathf.Cos(sputteringTurbulenceRate*Time.time)
			);
			track2.pitch += sputteringTurbulenceAmount2*Mathf.Sin(
				sputteringTurbulenceAmount1*Mathf.Cos(sputteringTurbulenceRate*Time.time)
			);

		}

		//if (brakeAmount == 1 && xTilt > 0) {

		//	driftSFX.Play();

			//}

	}



}