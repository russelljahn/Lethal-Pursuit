using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

	protected Spaceship spaceship;
	protected new bool active = true;

	public virtual void Start() {

	}

	public virtual void Update() {

	}

	public virtual bool ShouldDrop() {
		return false;
	}

	public virtual bool IsEquippable() {
		return false;
	}

	public virtual void OnPickup(Spaceship spaceship) {
		this.spaceship = spaceship;
		Debug.Log (this.gameObject.name + " was picked up by: " + spaceship.name);
	}

	public virtual void OnDrop() {
		Debug.Log (this.gameObject.name + " was dropped up by: " + spaceship.name);
	}

	public void SetActive(bool active) {
		this.active = active;
	}
	
}
