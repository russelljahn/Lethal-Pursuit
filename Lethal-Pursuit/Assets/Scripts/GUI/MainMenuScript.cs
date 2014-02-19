using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public GameObject MainPanel;
	public GameObject ModeSelect;
	
	public void Start() {
		MainPanel.SetActive(true);
		ModeSelect.SetActive(false);
	}

	public void OnExitClick() {
		Debug.Log("Exit Clicked");
		Application.Quit();
	}
	
	public void OnOptionsClick() {
		Debug.Log("Options Clicked");
	}
	
	public void OnStartClick() {
		Debug.Log("Start Clicked");
		ModeSelect.SetActive(true);
		MainPanel.SetActive(false);
	}
	
	public void OnMultiplayerClick() {
		Debug.Log("Multiplayer Clicked");
	}
	
	public void OnSingleplayerClick() {
		Debug.Log("Singleplayer Clicked");
	}
	
	public void OnTutorialClick() {
		Debug.Log("Tutorial Clicked");
		Application.LoadLevel("Tutorial");
	}
	
	public void OnReturnClick() {
		MainPanel.SetActive(true);
		ModeSelect.SetActive(false);
	}
}
