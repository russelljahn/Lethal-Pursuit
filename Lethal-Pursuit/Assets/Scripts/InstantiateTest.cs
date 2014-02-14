using UnityEngine;
using System.Collections;

public class InstantiateTest : MonoBehaviour {

	public string nameOfSpaceship = "Spaceships/Spaceship02";
	public GameObject objectToClone;
	public Transform spawnPoint;

	// Use this for initialization
	void Start () {

		GameObject spaceship = GameObject.Instantiate(
			Resources.Load (nameOfSpaceship),
			spawnPoint.position, 
			Quaternion.identity
		) as GameObject;

		SpaceshipCamera cam = GameObject.FindWithTag("MainCamera").GetComponent<SpaceshipCamera>();
		cam.SetSpaceship(spaceship.GetComponent<Spaceship>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
