using UnityEngine;
using System.Collections;
using InControl;



public class SpaceshipEffects : SpaceshipComponent {


	public ParticleSystem leftBoosterFlames;
	public ParticleSystem rightBoosterFlames;
	public ParticleSystem leftBoosterSmoke;
	public ParticleSystem rightBoosterSmoke;
	public float boostParticleEmissionRate = 254f;
	public float brakeParticleEmissionRate = 50f;
	public float boostParticleEmissionSize = 2.5f;
	public float brakeParticleEmissionSize = 1.5f;
	public float boostSmokeEmissionRate = 12.32f;
	public float brakeSmokeEmissionRate = 4f;



	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void HandleParticles() {

		float boostAmount = InputManager.ActiveDevice.RightTrigger.Value;
		float brakeAmount = InputManager.ActiveDevice.LeftTrigger.Value;
		
		if (brakeAmount == 0 && boostAmount > 0) {
			leftBoosterFlames.emissionRate =  boostParticleEmissionRate;
			rightBoosterFlames.emissionRate = boostParticleEmissionRate;
			leftBoosterFlames.startSize =  boostParticleEmissionSize;
			rightBoosterFlames.startSize = boostParticleEmissionSize;
			//			leftBoosterSmoke.emissionRate = boostSmokeEmissionRate;
			//			rightBoosterSmoke.emissionRate = boostSmokeEmissionRate;
			
			
		}
		else {
			leftBoosterFlames.emissionRate = brakeParticleEmissionRate;
			rightBoosterFlames.emissionRate = brakeParticleEmissionRate;
			leftBoosterFlames.startSize = brakeParticleEmissionSize;
			rightBoosterFlames.startSize = brakeParticleEmissionSize;
			//			leftBoosterSmoke.emissionRate = brakeSmokeEmissionRate;
			//			rightBoosterSmoke.emissionRate = brakeSmokeEmissionRate;
			
		}
		
	}


}
