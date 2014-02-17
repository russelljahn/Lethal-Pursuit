using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {


	private float startTime;
	public string textTime; 
	public float guiTime;
	public float minutes;
	public float seconds;
	public float ms;

	
	void Awake() {
		
		startTime = Time.time;
		
	}


	void Update(){
		//GetComponent<UILabel>().text = textTime; causes lag for some reason

		guiTime = Time.time - startTime;
		
		minutes = guiTime / 60;
		seconds = guiTime % 60;
		ms = (guiTime * 100) % 100;
		textTime = string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, ms); 

		textTime = guiTime.ToString();

	}
	
	void OnTriggerExit(){ //finish
		
	}

}