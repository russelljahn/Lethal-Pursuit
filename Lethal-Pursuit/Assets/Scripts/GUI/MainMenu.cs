using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	private static string gameType = "CS354T-Galacticats-LP";

	public GameObject titlePanel;
	public GameObject OptionsPanel;
	public GameObject modeSelectPanel;
	public GameObject multiplayerPanel;
	public GameObject vehicleSelectPanel;
	public GameObject mapSelectPanel;
	public GameObject LobbyPanel;
	public GameObject loadingPanel;
	
	public UIButton StartServerButton;
	public UIButton JoinServerButton;
	public UIButton RefreshButton;
	public UIButton LaunchButton;

	public float loadingPanelFadeTime = 0.5f;

	public  GameObject[] ServerButtons;
	public  UILabel[] 	 ButtonLabels;
	private HostData[]   hostdata;
	
	private bool refreshClicked = false;
	private static int lastLevelPrefix;

	private bool serverStarted = false;
	private string chosenShip  = null;
	private string chosenLevel = null;
	private bool tutorial = false;

	public string vehicle1Filepath = "Spaceships/Buzz";
	public string vehicle2Filepath = "Spaceships/Magneto II";
	public string vehicle3Filepath = "Spaceships/Patriot 69Z";

	public string tutorialFilename = "Tutorial";
	public string level1Filename = "Highway";
	
	
	public void Start() {

		HideAllMenus();
		titlePanel.SetActive(true);
		
		StartServerButton.isEnabled = true;
		JoinServerButton.isEnabled  = true;
		RefreshButton.isEnabled     = false;
		LaunchButton.isEnabled      = false;

		for(int i=0; i<ServerButtons.Length; i++) {
			ServerButtons[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (NetworkManager.IsServerListReady() && refreshClicked) {
			Debug.Log("Refreshing......");
			refreshClicked = false;
			OnServerListReady();	
		}
	}

	void HideAllMenus() {
		titlePanel.SetActive(false);
		OptionsPanel.SetActive(false);
		modeSelectPanel.SetActive(false);
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(false);
		mapSelectPanel.SetActive(false);
		LobbyPanel.SetActive(false);
		loadingPanel.SetActive(false);
	}



	public void OnExitClick() {
		Debug.Log("Exit Clicked");
		LevelManager.Quit();
	}
	
	public void OnOptionsClick() {
		Debug.Log("Options Clicked");
		titlePanel.SetActive(false);
		OptionsPanel.SetActive(true);
	}
	
	public void OnStartClick() {
		Debug.Log("Start Clicked");
		titlePanel.SetActive(false);
		modeSelectPanel.SetActive(true);
	}
	
	public void OnMultiplayerClick() {
		Debug.Log("Multiplayer Clicked");
		NetworkManager.SetSinglePlayer(false);
		
		modeSelectPanel.SetActive(false);
		multiplayerPanel.SetActive(true);
	}
	
	public void OnSingleplayerClick() {
		Debug.Log("Singleplayer Clicked");
		NetworkManager.SetSinglePlayer(true);
		tutorial = false;

		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	public void OnTutorialClick() {
		Debug.Log("Tutorial Clicked");
		NetworkManager.SetSinglePlayer(true);
		tutorial = true;

		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}

	public void OnReturnClick() {
		titlePanel.SetActive(true);
		OptionsPanel.SetActive(false);
		modeSelectPanel.SetActive(false);
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(false);
		mapSelectPanel.SetActive(false);
		LobbyPanel.SetActive(false);
		loadingPanel.SetActive(false);
		
		JoinServerButton.isEnabled = true;
		RefreshButton.isEnabled = false;
		
		if (serverStarted) {
			NetworkManager.ServerCleanup();
		}
		
		serverStarted = false;
	}

	public void OnStartServerClick() {
		NetworkManager.StartServer();
		JoinServerButton.isEnabled = false;
		
		multiplayerPanel.SetActive(false);
		mapSelectPanel.SetActive(true);
		
		LaunchButton.isEnabled = true;
		serverStarted = true;
	}

	public void OnJoinServerClick() {
		RefreshButton.isEnabled = true;

		NetworkManager.RefreshHostList();
		refreshClicked = true;
		
		LaunchButton.isEnabled = false;
	}
	
	private void OnServerListReady() {
	
		hostdata = NetworkManager.GetHostData();
		
		for (int i=0; i<ServerButtons.Length; ++i) {
			ServerButtons[i].SetActive(false);
		}
		
		for (int i=0; i<hostdata.Length && i<ServerButtons.Length; ++i) {
			ServerButtons[i].SetActive(true);
			ButtonLabels[i].text = hostdata[i].gameName;
		}
	}

	public void OnRefreshClick() {
		NetworkManager.RefreshHostList();
		refreshClicked = true;
	}

	public void OnServer1Click() {
		NetworkManager.JoinServer(0);
		
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}

	public void OnServer2Click() {
		NetworkManager.JoinServer(1);
		
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}

	public void OnServer3Click() {
		NetworkManager.JoinServer(2);
		
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}

	public void OnServer4Click() {
		NetworkManager.JoinServer(3);
		
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}
	
	public void OnVehicle1Click() {
		
		LevelManager.SetSpaceship(vehicle1Filepath);
		NetworkManager.SetShip(vehicle1Filepath);

		vehicleSelectPanel.SetActive(false);
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			LobbyPanel.SetActive(true);
		}
	}
	
	public void OnVehicle2Click() {
		
		LevelManager.SetSpaceship(vehicle2Filepath);
		NetworkManager.SetShip(vehicle2Filepath);

		vehicleSelectPanel.SetActive(false);
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			LobbyPanel.SetActive(true);
		}
	}
	
	public void OnVehicle3Click() {
		
		LevelManager.SetSpaceship(vehicle3Filepath);
		NetworkManager.SetShip(vehicle3Filepath);

		vehicleSelectPanel.SetActive(false);
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			LobbyPanel.SetActive(true);
		}
	}
	
	public void OnMap1Click() {
		
		//Record level name here
		chosenLevel = "Tutorial";

		mapSelectPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}
	
	public void OnMap2Click() {
		
		//Record level name here
		chosenLevel = "Tutorial";

		mapSelectPanel.SetActive(false);
		vehicleSelectPanel.SetActive(true);
	}

	public void OnLaunchClick() {
		networkView.RPC("SwitchLoad", RPCMode.All);
		networkView.RPC("LevelLoader", RPCMode.All);
		//LevelManager.LoadLevel(LevelManager.LEVEL.TUTORIAL);
	}
	
	[RPC]
	private void SwitchLoad(){
		LobbyPanel.SetActive(false);
		loadingPanel.SetActive(true);
	}
	
	[RPC]
	private void LevelLoader() {
		LevelManager.NetworkLoadLevel("Tutorial", 1);	
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
