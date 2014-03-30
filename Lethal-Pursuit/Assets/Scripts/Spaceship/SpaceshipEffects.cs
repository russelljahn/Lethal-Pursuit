using UnityEngine;
using System.Collections;
using InControl;



public class SpaceshipEffects : SpaceshipComponent {

	public PlaygroundParticles [] particlesBasedOnSpeed;
	public float minEmissionRate = 0.15f;
	public float maxEmmissionRate = 1.00f;
	

	void Update() {
		float speedRatio = spaceship.currentBoostVelocity/spaceship.maxBoostVelocity;
		for (int i = 0; i < particlesBasedOnSpeed.Length; ++i) {
			PlaygroundParticles particleSystem = particlesBasedOnSpeed[i];
			particleSystem.emissionRate = minEmissionRate + (maxEmmissionRate-minEmissionRate)*speedRatio;
		}
	}


}
