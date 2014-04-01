using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {


	private float startTime;
	private UILabel label;
	
	void Start() {
		
		startTime = Time.time;
		label = GetComponent<UILabel>();
	}


	void Update() {

		float currentTime = Time.time - startTime;
		float minutes = currentTime / 60;
		float seconds = currentTime % 60;
		float ms = (currentTime * 100) % 100;

		label.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, ms); 


	}


}