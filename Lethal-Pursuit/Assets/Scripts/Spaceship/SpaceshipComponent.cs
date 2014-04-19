using UnityEngine;
using System;
using System.Collections;

public class SpaceshipComponent : MonoBehaviour {


	public Spaceship spaceship; 


	/* Tilt of left analogue stick every frame. */
	protected float xTiltLeftStick {
		get {
			return spaceship.xTiltLeftStick;
		}
	}
	protected float yTiltLeftStick {
		get {
			return spaceship.yTiltLeftStick;
		}
	} 
	/* Tilt of right analogue stick every frame. */
	protected float xTiltRightStick {
		get {
			return spaceship.xTiltRightStick;
		}
	}
	protected float yTiltRightStick {
		get {
			return spaceship.yTiltRightStick;
		}
	} 
	/* Is player hitting the shoot button right now? */
	protected bool shooting {
		get {
			return spaceship.shooting;
		}
	}
	protected bool boosting {
		get {
			return spaceship.boosting;
		}
	}
	protected bool braking {
		get {
			return spaceship.braking;
		}
	}
	protected bool drifting {
		get {
			return spaceship.drifting;
		}
	}
	protected bool idle {
		get {
			return spaceship.idle;
		}
	}
	protected bool swappingWeapon {
		get {
			return spaceship.swappingWeapon;
		}
	}
	protected EquipType equippedItem {
		get {
			return spaceship.equippedItem;
		}
		set {
			spaceship.equippedItem = value;
		}
	}
	protected GameplayManager gameplayManager {
		get {
			return spaceship.gameplayManager;
		}
	}
	protected GameObject spaceshipModelRoot {
		get {
			return spaceship.spaceshipModelPitchYaw;
		}
	}
	protected GameObject spaceshipModel {
		get {
			return spaceship.spaceshipModelRoll;
		}
	}
	protected GameObject spaceshipShell {
		get {
			return spaceship.spaceshipShell;
		}
	}
	protected Vector3 forward {
		get {
			return spaceship.forward;
		}
	}
	protected Vector3 right {
		get {
			return spaceship.right;
		}
	}
	protected float currentBoostVelocity {
		get {
			return spaceship.currentBoostVelocity;
		}
		set {
			spaceship.currentBoostVelocity = value;
		}
	}
	protected float maxBoostVelocity {
		get {
			return spaceship.maxBoostVelocity;
		}
	}


	public virtual void Start () {
		if (spaceship == null) {
			throw new Exception("spaceship is null for SpaceshipComponent in: " + this.gameObject.name);
		}
	}

	
	public virtual void Update () {
		;
	}




}
