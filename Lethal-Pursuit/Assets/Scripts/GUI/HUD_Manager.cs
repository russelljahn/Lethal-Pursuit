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


	// Use this for initialization
	void Start () {
		currentLevel = LevelManager.GetLoadedLevel();

		isTutorialLevel = currentLevel.name.Equals("Tutorial");
		tutorialGui.SetActive(isTutorialLevel);

		menuGui.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		pressedStartLastFrame = pressingStart;
		pressingStart = Input.GetKey(KeyCode.Escape);
		releasedStart = !pressingStart && pressedStartLastFrame;


		if (releasedStart) {
			if (menuGui.activeInHierarchy) {
				HideMenu();
			}
			else {
				DisplayMenu();
			}
		}
	}


	public void DisplayMenu() {
//		Time.timeScale = 0.0f; // Locks up currently if used
		spaceship.enabled = false;
		menuGui.SetActive(true);
	}


	public void HideMenu() {
//		Time.timeScale = 1.0f; // Locks up currently if used
		spaceship.enabled = true;
		menuGui.SetActive(false);
	}


	public void LoadMainMenu() {
		LevelManager.LoadMainMenu();
	}

	
}
