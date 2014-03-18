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

	void ShowLoadingScreen() {
		loadingPanel.SetActive(true);
		TweenAlpha alphaTween = loadingPanel.AddComponent<TweenAlpha>();
		alphaTween.from = 0.0f;
		alphaTween.to = 1.0f;
		alphaTween.duration = loadingPanelFadeTime;
		alphaTween.animationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, loadingPanelFadeTime, 1.0f);
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

		ShowLoadingScreen();
		NetworkManager.SetSinglePlayer(true);
		StartCoroutine(OnSingleplayerClickHelper());
	}

	private IEnumerator OnSingleplayerClickHelper() {
		yield return new WaitForSeconds(loadingPanelFadeTime);
		LevelManager.LoadLevel("Highway");
	}
	
	public void OnTutorialClick() {
		Debug.Log("Tutorial Clicked");
		NetworkManager.SetSinglePlayer(true);

		ShowLoadingScreen();
		chosenShip = "Spaceships/Patriot 69Z";
//		NetworkManager.SetShip(chosenShip);
		StartCoroutine(OnTutorialClickHelper());
	}

	private IEnumerator OnTutorialClickHelper() {
		yield return new WaitForSeconds(loadingPanelFadeTime);
		LevelManager.LoadLevel("Tutorial");
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
		
		//Record ship name here
		chosenShip = "Spaceships/Buzz";
		NetworkManager.SetShip(chosenShip);

		vehicleSelectPanel.SetActive(false);
		LobbyPanel.SetActive(true);
	}
	
	public void OnVehicle2Click() {
		
		//Record ship name here
		chosenShip = "Spaceships/Magneto II";
		NetworkManager.SetShip(chosenShip);

		vehicleSelectPanel.SetActive(false);
		LobbyPanel.SetActive(true);
	}
	
	public void OnVehicle3Click() {
		
		//Record ship name here
		chosenShip = "Spaceships/Patriot 69Z";
		NetworkManager.SetShip(chosenShip);

		vehicleSelectPanel.SetActive(false);
		LobbyPanel.SetActive(true);
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
