using UnityEngine;
using System;
using System.Collections;

public class SpaceshipComponent : MonoBehaviour {


	public Spaceship spaceship; 


	/* Tilt of analogue stick every frame. */
	protected float xTilt {
		get {
			return spaceship.xTilt;
		}
	}
	protected float yTilt {
		get {
			return spaceship.yTilt;
		}
	} 
	/* Amount of boost button pressed down. */
	protected float boostAmount {
		get {
			return spaceship.boostAmount;
		}
	}
	/* Amount of brake button pressed down. */
	protected float brakeAmount {
		get {
			return spaceship.brakeAmount;
		}
	}
	/* Is player hitting the shoot button right now? */
	protected bool currentlyShooting {
		get {
			return spaceship.currentlyShooting;
		}
	}
		
	protected GameplayManager gameplayManager {
		get {
			return spaceship.gameplayManager;
		}
	}
	protected GameObject spaceshipModel {
		get {
			return spaceship.spaceshipModel;
		}
	}


	


	// Use this for initialization
	public virtual void Start () {
		if (spaceship == null) {
			throw new Exception("spaceship is null for SpaceshipComponent in " + this.gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
