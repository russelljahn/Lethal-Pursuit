using UnityEngine;
using System.Collections;
using InControl;


[RequireComponent(typeof(NetworkView))]
public class SpaceshipEffects : SpaceshipComponent {

	public PlaygroundParticles [] particlesBasedOnSpeed;
	public float minEmissionRate = 0.15f;
	public float maxEmmissionRate = 1.00f;
	

	public override void Update() {

		float speedRatio = spaceship.currentBoostVelocity/spaceship.maxBoostVelocity;
		
		if (NetworkManager.IsSinglePlayer() || networkView.isMine) {
			for (int i = 0; i < particlesBasedOnSpeed.Length; ++i) {
				PlaygroundParticles particleSystem = particlesBasedOnSpeed[i];
				particleSystem.emissionRate = minEmissionRate + (maxEmmissionRate-minEmissionRate)*speedRatio;
			}
		}
		if (!NetworkManager.IsSinglePlayer() && networkView.isMine && spaceship.isVisible) {
			networkView.RPC("NetworkUpdateEffects", RPCMode.OthersBuffered, speedRatio);
		}
	}


	[RPC]
	private void NetworkUpdateEffects(float speedRatio) {
		for (int i = 0; i < particlesBasedOnSpeed.Length; ++i) {
			PlaygroundParticles particleSystem = particlesBasedOnSpeed[i];
			particleSystem.emissionRate = minEmissionRate + (maxEmmissionRate-minEmissionRate)*speedRatio;
		}
	}


}
