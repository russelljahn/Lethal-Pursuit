using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	Spaceship spaceship;

	public virtual void Start() {

	}

	public virtual void Update() {

	}

	public virtual bool ShouldDrop() {
		return false;
	}

	public virtual void OnPickup(Spaceship spaceship) {
		this.spaceship = spaceship;
	}

	public virtual void OnDrop() {

	}
	
}
