using InControl;
using UnityEngine;
using System.Collections;

public class HUD_Manager : MonoBehaviour {

	public Spaceship spaceship;

	public bool pressingStart;
	public bool releasedStart;

	private bool pressedStartLastFrame;
	private Level currentLevel;

	private bool isTutorialLevel;

	public GameObject tutorialGui;
	public GameObject menuGui;
	public GameObject controlsGui;
	public GameObject quitConfirmationGui;
	public GameObject raceOverGui;
	


	// Use this for initialization
	void Start () {
		spaceship = GameplayManager.spaceship;
		currentLevel = LevelManager.GetLoadedLevel();

		isTutorialLevel = currentLevel.name.Equals("Tutorial");
		tutorialGui.SetActive(isTutorialLevel);

		menuGui.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		pressedStartLastFrame = pressingStart;
		pressingStart = InputManager.ActiveDevice.GetControl(InputControlType.Start);
		releasedStart = !pressingStart && pressedStartLastFrame;
		
		if (releasedStart) {
			if (menuGui.activeInHierarchy) {
				HideMenu();
			}
			else if (!tutorialGui.activeInHierarchy && !controlsGui.activeInHierarchy && !quitConfirmationGui.activeInHierarchy && !raceOverGui.activeInHierarchy) {
				DisplayMenu();
			}
		}
	}


	public void DisplayMenu() {
		spaceship.enabled = false;
		menuGui.SetActive(true);
	}


	public void HideMenu() {
		spaceship.enabled = true;
		menuGui.SetActive(false);
	}


	public void DisplayQuitConfirmation() {
		menuGui.SetActive(false);
		quitConfirmationGui.SetActive(true);
	}
	
	
	public void HideQuitConfirmation() {
		menuGui.SetActive(true);
		quitConfirmationGui.SetActive(false);
	}


	public void DisplayControls() {
		menuGui.SetActive(false);
		controlsGui.SetActive(true);
	}
	
	
	public void HideControls() {
		menuGui.SetActive(true);
		controlsGui.SetActive(false);
	}


	public void DisplayRaceOver() {
		spaceship.enabled = false;
		raceOverGui.SetActive(true);
	}
	
	
	public void HideRaceOver() {
		spaceship.enabled = true;
		raceOverGui.SetActive(false);
	}


	public void ReplayTrack() {
		if(NetworkManager.IsSinglePlayer()) {
			LevelManager.ReloadLevel();
		}
		else {
			networkView.RPC("NetworkReplayTrack", RPCMode.All);
		}
	}

	[RPC]
	private void NetworkReplayTrack() {
		LevelManager.ReloadLevel();
	}

	public void LoadMainMenu() {
		if(Network.isServer) {
			NetworkManager.ServerCleanup();
		}
		LevelManager.LoadMainMenu();
	}

	
}
