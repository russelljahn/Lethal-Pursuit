using UnityEngine;
using System.Collections;
using InControl;



public class SpaceshipEffectsYeOlde : SpaceshipComponent {
	
	//public ParticleSystem speedBeams;
	//public float speedBeamsMaxOpacity = 0.20f;
	
	
	public ParticleSystem leftBoosterFlames; //thrusters
	public ParticleSystem rightBoosterFlames;
	//public ParticleSystem miniThruster1;
	//public ParticleSystem miniThruster2;
	//public ParticleSystem miniThruster3;
	//public ParticleSystem miniThruster4;
//	public ParticleSystem vertMiniL;
//	public ParticleSystem vertMiniR;
	
	
	//trails
	public TrailRenderer trail1;
	public TrailRenderer trail2;
	public TrailRenderer trail3;
	public TrailRenderer trail4;
//	public ParticleSystem leftBoosterSmoke;
//	public ParticleSystem rightBoosterSmoke;
	
	public Color boosterFlamesBoostColor; //colors
	public Color boosterFlamesBrakeColor;
	
	public float boostParticleEmissionRate = 254f;
	public float brakeParticleEmissionRate = 50f;
	public float boostParticleEmissionSize = 2.5f;
	public float miniBoostParticleEmissionSize = 2.8f;
	public float miniBrakeParticleEmissionSize = 0.8f;
	public float brakeParticleEmissionSize = 1.5f;
	public float boostSmokeEmissionRate = 12.32f;
	public float brakeSmokeEmissionRate = 4f;
	
	
	
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		boosterFlamesBoostColor = leftBoosterFlames.startColor;
		
	}
	
	
	
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
		HandleParticles();
	}
	
	
	
	
	void HandleParticles() {
		
//		Color newSpeedBeamsColor = speedBeams.startColor;
//		newSpeedBeamsColor.a = (spaceship.currentBoostVelocity / spaceship.maxBoostVelocity)*speedBeamsMaxOpacity;
//		speedBeams.startColor = newSpeedBeamsColor;
		
		
//		if (xTilt > 0) {
//			//Vector3 newVelocity = new Vector3 (1.09f, yTilt, xTilt);
//			Vector3 newVelocity = new Vector3 (2.09f, xTilt, Mathf.Clamp (spaceship.transform.localRotation.y, 0, 1)); 
//			emitter.localVelocity = newVelocity;
//			trail1.localVelocity = newVelocity;
//			trail2.localVelocity = newVelocity;
//			trail3.localVelocity = newVelocity;
//			trail4.localVelocity = newVelocity;
//			
//		} 
//		
//		if (xTilt < 0) {
//			//Vector3 newVelocity = new Vector3 (1.09f, yTilt, xTilt);
//			Vector3 newVelocity = new Vector3 (2.09f, xTilt, spaceship.transform.rotation.y); 
//			emitter.localVelocity = newVelocity;
//			trail1.localVelocity = newVelocity;
//			trail2.localVelocity = newVelocity;
//			trail3.localVelocity = newVelocity;
//			trail4.localVelocity = newVelocity;
//		}
		
		//		if (enforceHeightLimit || heightAboveGround > fractionOfHeightLimitToBeginSputtering*spaceship.heightLimit) {
		//			leftBoosterFlames.startColor = boosterFlamesBrakeColor;
		//			rightBoosterFlames.startColor = boosterFlamesBrakeColor;
		//			miniThruster1.startColor = boosterFlamesBrakeColor;
		//			miniThruster2.startColor = boosterFlamesBrakeColor;
		//			miniThruster3.startColor = boosterFlamesBrakeColor;
		//			miniThruster4.startColor = boosterFlamesBrakeColor;
		//
		//		
		//
		//			
		//			leftBoosterFlames.emissionRate = brakeParticleEmissionRate;
		//			rightBoosterFlames.emissionRate = brakeParticleEmissionRate;
		//			leftBoosterFlames.startSize = brakeParticleEmissionSize;
		//			rightBoosterFlames.startSize = brakeParticleEmissionSize;
		//			miniThruster1.startSize = miniBrakeParticleEmissionSize;
		//			miniThruster2.startSize = miniBrakeParticleEmissionSize;
		//			miniThruster3.startSize = miniBrakeParticleEmissionSize;
		//			miniThruster4.startSize = miniBrakeParticleEmissionSize;
		//	
		//			
		//
		//	
		//		}
		if (spaceship.currentBoostVelocity < 100) {
			leftBoosterFlames.startColor = boosterFlamesBoostColor;
			rightBoosterFlames.startColor = boosterFlamesBoostColor;
//			miniThruster1.startColor = boosterFlamesBoostColor;
//			miniThruster2.startColor = boosterFlamesBoostColor;
//			miniThruster3.startColor = boosterFlamesBoostColor;
//			miniThruster4.startColor = boosterFlamesBoostColor;
//			
			leftBoosterFlames.emissionRate =  boostParticleEmissionRate;
			rightBoosterFlames.emissionRate = boostParticleEmissionRate;
			leftBoosterFlames.startSize =  boostParticleEmissionSize;
			rightBoosterFlames.startSize = boostParticleEmissionSize;
			//			leftBoosterSmoke.emissionRate = boostSmokeEmissionRate;
			//			rightBoosterSmoke.emissionRate = boostSmokeEmissionRate;
//			miniThruster1.startSize = miniBoostParticleEmissionSize;
//			miniThruster2.startSize = miniBoostParticleEmissionSize;
//			miniThruster3.startSize = miniBoostParticleEmissionSize;
//			miniThruster4.startSize = miniBoostParticleEmissionSize;
			trail1.enabled = false;
			trail2.enabled = false;
			trail3.enabled = false;
			trail4.enabled = false;
			
			
			
			
		}
		else {
			leftBoosterFlames.startColor = boosterFlamesBrakeColor;
			rightBoosterFlames.startColor = boosterFlamesBrakeColor;
			
			leftBoosterFlames.emissionRate = brakeParticleEmissionRate;
			rightBoosterFlames.emissionRate = brakeParticleEmissionRate;
			leftBoosterFlames.startSize = brakeParticleEmissionSize;
			rightBoosterFlames.startSize = brakeParticleEmissionSize;
			//			leftBoosterSmoke.emissionRate = brakeSmokeEmissionRate;
			//			rightBoosterSmoke.emissionRate = brakeSmokeEmissionRate;
//			miniThruster1.startSize = miniBrakeParticleEmissionSize;
//			miniThruster2.startSize = miniBrakeParticleEmissionSize;
//			miniThruster3.startSize = miniBrakeParticleEmissionSize;
//			miniThruster4.startSize = miniBrakeParticleEmissionSize;
			trail1.enabled = false;
			trail2.enabled = false;
			trail3.enabled = false;
			trail4.enabled = false;
//			
//		}
		
//		if (spaceship.currentVelocity < 100) {
//			vertMiniL.startSize = 2;
//			vertMiniR.startSize = 2;
//		}
//		else {
//			vertMiniR.startSize = 0;
//			vertMiniL.startSize = 0;
//			
//		}
//	}
	
	
}
}
}