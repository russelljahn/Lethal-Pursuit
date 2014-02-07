using UnityEngine;
using InControl;
using System.Collections;

public class SpaceshipAudio : SpaceshipComponent {

	public AudioSource track1;
	public AudioSource track2;

	public AudioClip engine4;
	public AudioClip engine1; 
	public AudioClip engine3; 
	
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

		track2.clip = engine4;
		track2.loop = true;
		track2.Play();
	}



	public override void Update () {
		base.Update();
						
		if (boostAmount > 0) {
			boostTime = Mathf.Min(maxBoostTime, boostTime+Time.deltaTime);
		}
				
		else {
			boostTime = Mathf.Max(0f, boostTime-Time.deltaTime);	
		}

				
		track1.pitch = maxPitchEngine1*boostTime / maxBoostTime;
		track2.pitch = maxPitchEngine1*boostTime / maxBoostTime;

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
	}



}