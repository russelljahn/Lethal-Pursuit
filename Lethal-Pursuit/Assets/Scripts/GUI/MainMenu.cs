using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public GameObject titlePanel;
	public GameObject backPanel;
	public GameObject optionsPanel;
	public GameObject modeSelectPanel;
	public GameObject vehicleSelectPanel;
	public GameObject mapSelectPanel;
	public GameObject multiplayerHubPanel;
	public GameObject lobbyPanel;
	public GameObject joinServerPanel;	
	
	public UIButton startServerButton;
	public UIButton joinServerButton;
	public UIButton refreshButton;
	public UIButton launchButton;
	public UILabel launchText;
	
	
	public  GameObject[] serverButtons;
	public  UILabel[] 	 buttonLabels;
	private HostData[]   hostdata;
	
	private bool refreshClicked = false;
	private static int lastLevelPrefix;
	
	private bool serverStarted = false;
	
	private bool tutorial = false;
	private bool client = false;
	
	public string vehicle1Filepath = "Spaceships/Buzz";
	public string vehicle2Filepath = "Spaceships/Magneto II";
	public string vehicle3Filepath = "Spaceships/Patriot 69Z";
	
	public string tutorialFilename = "Tutorial";
	public string level1Filename = "Arena";
	
	
	public void Start() {
		HideAllMenus();
		startServerButton.isEnabled = true;
		joinServerButton.isEnabled  = true;
		refreshButton.isEnabled     = false;
		launchButton.isEnabled      = false;
		
		for (int i = 0; i < serverButtons.Length; ++i) {
			serverButtons[i].SetActive(false);
		}
		OnTitleClick();
	}
	
	
	// Update is called once per frame
	void Update () {
		if (NetworkManager.IsServerListReady() && refreshClicked) {
			refreshClicked = false;
			OnServerListReady();	
		}
	}
	
	
	void HideAllMenus() {
		titlePanel.SetActive(false);
		optionsPanel.SetActive(false);
		modeSelectPanel.SetActive(false);
		vehicleSelectPanel.SetActive(false);
		mapSelectPanel.SetActive(false);
		multiplayerHubPanel.SetActive(false);
		lobbyPanel.SetActive(false);
		joinServerPanel.SetActive(false);
	}
	
	
	void HideBackButton() {
		backPanel.SetActive(false);
	}
	
	
	void ShowBackButton() {
		backPanel.SetActive(true);
	}
	
	
	
	public void OnClickBack() {
		
		Debug.Log("Server started status: " + serverStarted);
		
		if(serverStarted) {
			
			Debug.Log("Entered onMultiplayer reenabler");
			NetworkManager.ServerCleanup();
			joinServerButton.isEnabled = true;
			
			launchButton.isEnabled = false;
			refreshButton.isEnabled = false;
			serverStarted = false;
			
			Debug.Log("Status of join button: " + joinServerButton.isEnabled);
		}
		
		// Exit
		if (titlePanel.activeInHierarchy) {
			OnExitClick();
		}
		// Mode Select -> Title Screen
		else if (modeSelectPanel.activeInHierarchy) {
			HideAllMenus();
			titlePanel.SetActive(true);
			UILabel backButtonText = backPanel.GetComponentInChildren<UILabel>();
			backButtonText.text = "Exit";
		}
		// Options -> Mode Select
		else if (optionsPanel.activeInHierarchy) {
			OnModeSelectClick();
		}
		// Vehicle Select -> Mode Select (if Singleplayer)/MultiplayerHub (if Multiplayer)
		else if (vehicleSelectPanel.activeInHierarchy) {
			if (NetworkManager.IsSinglePlayer()) {
				OnModeSelectClick();
			}
			else {
				OnMultiplayerClick();
			}
		}
		// Map Select -> Vehicle Select
		else if (mapSelectPanel.activeInHierarchy) {
			
		}
		// MultiplayerHub -> Mode Select
		else if (multiplayerHubPanel.activeInHierarchy) {
			OnModeSelectClick();
		}
		// Lobby -> MultiplayerHub (if Multiplayer CreateServer)/JoinServer (if Multiplayer JoinServer)
		else if (lobbyPanel.activeInHierarchy) {
			if (client) {
				client = false;
				OnJoinServerClick();
			}
			else {
				OnMultiplayerClick();
			}
		}
		// JoinServer -> MultiplayerHub
		else if (joinServerPanel.activeInHierarchy) {
			OnMultiplayerClick();
		}
	}
	
	
	public void OnExitClick() {
		Debug.Log("Exit Clicked");
		LevelManager.Quit();
	}
	
	
	public void OnOptionsClick() {
		Debug.Log("Options Clicked");
		HideAllMenus();
		optionsPanel.SetActive(true);
	}
	
	
	public void OnTitleClick() {
		Debug.Log("Title Clicked");
		HideAllMenus();
		titlePanel.SetActive(true);
		
		UILabel backButtonText = backPanel.GetComponentInChildren<UILabel>();
		backButtonText.text = "Exit";
	}
	
	
	public void OnModeSelectClick() {
		Debug.Log("Mode Select Clicked");
		HideAllMenus();
		modeSelectPanel.SetActive(true);
		
		UILabel backButtonText = backPanel.GetComponentInChildren<UILabel>();
		backButtonText.text = "Back";
	}
	
	
	public void OnMultiplayerClick() {
		Debug.Log("Multiplayer Clicked");
		
		NetworkManager.SetSinglePlayer(false);
		tutorial = false;
		
		HideAllMenus();
		multiplayerHubPanel.SetActive(true);
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
	
	
	public void OnStartServerClick() {
		HideBackButton();
		
		NetworkManager.StartServer();
		joinServerButton.isEnabled = false;
		
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		
		launchButton.isEnabled = true;
		serverStarted = true;
		
		Debug.Log("Server started status: " + serverStarted);
		
		launchText.text = "Launch Game";
	}
	
	
	public void OnJoinServerClick() {
		HideBackButton();
		refreshButton.isEnabled = true;
		
		NetworkManager.RefreshHostList();
		refreshClicked = true;
		
		launchButton.isEnabled = false;
		
		HideAllMenus();
		joinServerPanel.SetActive(true);
		
		launchText.text = "Waiting On Host";
		client = true;
	}
	
	
	private void OnServerListReady() {
		hostdata = NetworkManager.GetHostData();
		
		for (int i = 0; i < serverButtons.Length; ++i) {
			serverButtons[i].SetActive(false);
		}
		
		for (int i = 0; i < hostdata.Length && i < serverButtons.Length; ++i) {
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
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnServer2Click() {
		NetworkManager.JoinServer(1);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnServer3Click() {
		NetworkManager.JoinServer(2);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnServer4Click() {
		NetworkManager.JoinServer(3);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnVehicle1Click() {
		LevelManager.SetSpaceship(vehicle1Filepath);
		
		HideAllMenus();
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			OnLobbyClick();
		}
	}
	
	
	public void OnVehicle2Click() {
		LevelManager.SetSpaceship(vehicle2Filepath);
		
		HideAllMenus();
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			OnLobbyClick();
		}
	}
	
	
	public void OnVehicle3Click() {
		LevelManager.SetSpaceship(vehicle3Filepath);
		
		HideAllMenus();
		if (NetworkManager.IsSinglePlayer()) {
			if (tutorial) {
				LevelManager.LoadLevel(tutorialFilename);
			}
			else {
				LevelManager.LoadLevel(level1Filename);
			}
		}
		else {
			OnLobbyClick();
		}
	}
	
	
	public void OnMap1Click() {
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnMap2Click() {
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
	}
	
	
	public void OnLobbyClick() {
		HideAllMenus();
		lobbyPanel.SetActive(true);
	}
	
	
	public void OnLaunchClick() {
		networkView.RPC("SwitchLoad", RPCMode.All);
		networkView.RPC("LevelLoader", RPCMode.All);
	}
	
	
	[RPC]
	private void SwitchLoad() {
		lobbyPanel.SetActive(false);
	}
	
	
	[RPC]
	private void LevelLoader() {
		LevelManager.NetworkLoadLevel(level1Filename, 1);	
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}