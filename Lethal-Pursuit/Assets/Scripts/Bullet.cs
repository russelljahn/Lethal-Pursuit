using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Vector3 direction;
	public float speed = 50f;
	public float timeUntilDeath = 5.0f;
	public bool prefab = true;


	// Use this for initialization
	void Start () {
		Debug.Log ("Bullet.transform.position: " + this.transform.position);
		direction.Normalize();
	}




	
	// Update is called once per frame
	void Update () {

		if (!prefab) {
			timeUntilDeath -= Time.deltaTime;
			if (timeUntilDeath <= 0) {
				GameObject.Destroy(this.gameObject);
			}

			this.rigidbody.MovePosition(this.transform.position + speed*direction*Time.deltaTime);
		}
	}




}
