using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class RaceManager : MonoBehaviour {


	SpaceshipRaceData [] datas;



	// Use this for initialization
	void Start () {
		SpaceshipRaceData [] spaceshipGameObjects = GameObject.FindObjectsOfType<SpaceshipRaceData>();
	}



	// Update is called once per frame
	void Update () {
//		spaceships.Sort(
//			(Spaceship lhs, Spaceship rhs) => lhs.CompareTo(rhs.transform.position.x)
//		);
	}



}
