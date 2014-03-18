using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	private static string gameType = "CS354T-Galacticats-LP";

	public GameObject titlePanel;
	public GameObject optionsPanel;
	public GameObject modeSelectPanel;
	public GameObject multiplayerPanel;
	public GameObject vehicleSelectPanel;
	public GameObject mapSelectPanel;
	public GameObject lobbyPanel;
	
	public UIButton startServerButton;
	public UIButton joinServerButton;
	public UIButton refreshButton;
	public UIButton launchButton;

	public float loadingPanelFadeTime = 0.5f;

	public  GameObject[] serverButtons;
	public  UILabel[] 	 buttonLabels;
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
		
		startServerButton.isEnabled = true;
		joinServerButton.isEnabled  = true;
		refreshButton.isEnabled     = false;
		launchButton.isEnabled      = false;

		for(int i=0; i<serverButtons.Length; i++) {
			serverButtons[i].SetActive(false);
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
		optionsPanel.SetActive(false);
		modeSelectPanel.SetActive(false);
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(false);
		mapSelectPanel.SetActive(false);
		lobbyPanel.SetActive(false);
	}
	
	public void OnExitClick() {
		Debug.Log("Exit Clicked");
		LevelManager.Quit();
	}
	
	public void OnOptionsClick() {
		Debug.Log("Options Clicked");
		titlePanel.SetActive(false);
		optionsPanel.SetActive(true);
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
		optionsPanel.SetActive(false);
		modeSelectPanel.SetActive(false);
		multiplayerPanel.SetActive(false);
		vehicleSelectPanel.SetActive(false);
		mapSelectPanel.SetActive(false);
		lobbyPanel.SetActive(false);
		
		joinServerButton.isEnabled = true;
		refreshButton.isEnabled = false;
		
		if (serverStarted) {
			NetworkManager.ServerCleanup();
		}
		
		serverStarted = false;
	}

	public void OnStartServerClick() {
		NetworkManager.StartServer();
		joinServerButton.isEnabled = false;
		
		multiplayerPanel.SetActive(false);
		mapSelectPanel.SetActive(true);
		
		launchButton.isEnabled = true;
		serverStarted = true;
	}

	public void OnJoinServerClick() {
		refreshButton.isEnabled = true;

		NetworkManager.RefreshHostList();
		refreshClicked = true;

		launchButton.isEnabled = false;
	}
	
	private void OnServerListReady() {
	
		hostdata = NetworkManager.GetHostData();
		
		for (int i=0; i<serverButtons.Length; ++i) {
			serverButtons[i].SetActive(false);
		}
		
		for (int i=0; i<hostdata.Length && i<serverButtons.Length; ++i) {
			serverButtons[i].SetActive(true);
			buttonLabels[i].text = hostdata[i].gameName;
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
			lobbyPanel.SetActive(true);
		}
	}
	
	public void OnVehicle2Click() {
		
		LevelManager.SetSpaceship(vehicle2Filepath);

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
			lobbyPanel.SetActive(true);
		}
	}
	
	public void OnVehicle3Click() {
		
		LevelManager.SetSpaceship(vehicle3Filepath);

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
			lobbyPanel.SetActive(true);
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
		lobbyPanel.SetActive(false);
	}
	
	[RPC]
	private void LevelLoader() {
		LevelManager.NetworkLoadLevel("Tutorial", 1);	
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}
