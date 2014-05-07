using UnityEngine;
using System;
using System.Collections;
using InControl;

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
	public GameObject creditsPanel;	


	public UIButton[] currentPanelButtons;
	public int selectedButtonIndex;
	
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
	
	public string vehicle1Filepath = "Spaceships/Littlefoot";
	public string vehicle2Filepath = "Spaceships/Helldiver";
	public string vehicle3Filepath = "Spaceships/Mufasa";
	public string vehicle4Filepath = "Spaceships/Dauntless";
	
	public string tutorialFilename = "Tutorial";
	public string level1Filename = "Arena";
	
	private int playersReady = 1;
	private UIRoot uiRoot;
	public float buttonNormalOpacity = 0.5f;
	public float buttonTweenDuration = 0.04f;
	
	public void Start() {
		RegisterEventHandlers();

		HideAllMenus();
		startServerButton.isEnabled = true;
		joinServerButton.isEnabled  = true;
		refreshButton.isEnabled     = false;
		launchButton.isEnabled      = false;
		
		for (int i = 0; i < serverButtons.Length; ++i) {
			serverButtons[i].SetActive(false);
		}
		OnTitleClick();
		StartCoroutine(ReloadCurrentPanelButtons());
	}
	
	
	// Update is called once per frame
	void Update () {
		if (NetworkManager.IsServerListReady() && refreshClicked) {
			refreshClicked = false;
			OnServerListReady();	
		}

		bool pressedConfirm = InputManager.ActiveDevice.Action1.WasPressed || InputManager.ActiveDevice.GetControl(InputControlType.Start).WasPressed;
		bool releasedConfirm = InputManager.ActiveDevice.Action1.WasReleased || InputManager.ActiveDevice.GetControl(InputControlType.Start).WasReleased;
		bool releasedCancel = InputManager.ActiveDevice.Action2.WasReleased;
		bool releasedUp = InputManager.ActiveDevice.DPadUp.WasReleased;
		bool releasedDown = InputManager.ActiveDevice.DPadDown.WasReleased;
		bool releasedLeft = InputManager.ActiveDevice.DPadLeft.WasReleased;
		bool releasedRight = InputManager.ActiveDevice.DPadRight.WasReleased;


		if (pressedConfirm) {
			GetSelectedButton().SendMessage("OnPress", true);
		}
		else if (releasedConfirm) {
			GetSelectedButton().SendMessage("OnClick");
		}
		else if (releasedCancel) {
			OnClickBack();
			StartCoroutine(ReloadCurrentPanelButtons());
			backPanel.SendMessage("OnHover", false);
		}
		else if (vehicleSelectPanel.activeInHierarchy) {
			if (releasedUp || releasedDown || releasedLeft || releasedRight) {
				GetSelectedButton().SendMessage("OnHover", false);	
				VehicleScreenSelectNextButton();
				GetSelectedButton().SendMessage("OnHover", true);	
			}
		}
		else if (releasedDown) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectNextButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}
		else if (releasedUp) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectPreviousButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}
		else if (releasedLeft) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectNextButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}
		else if (releasedRight) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectPreviousButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}
	}


	// Make this script subscribe to ui events from buttons in the scene
	void RegisterEventHandlers() {
		uiRoot = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIRoot>();
		UIButton [] buttons = uiRoot.gameObject.GetComponentsInChildren<UIButton>(true);
		for (int i = 0; i < buttons.Length; ++i) {
			UIButton button = buttons[i];
			Debug.Log ("Adding listeners for button: " + button.gameObject);
			UIEventListener.Get(button.gameObject).onClick += OnButtonClick;
			UIEventListener.Get(button.gameObject).onHover += OnButtonHover;

			Color normalColor = button.defaultColor;
			normalColor.a = buttonNormalOpacity;
			button.defaultColor = normalColor;
			button.duration = buttonTweenDuration;
		}
	}


	public void OnButtonClick(GameObject source) {
		Debug.Log ("Clicked on: " + source);
		StartCoroutine(ReloadCurrentPanelButtons());
	}


	public void OnButtonHover(GameObject source, bool isOver) {
//		Debug.Log ("source: " + source);
		if (isOver) {
			SetSelectedButton(source.GetComponentInChildren<UIButton>());
//			Debug.Log ("MainMenu: Got 'isOver=true' OnHover event from: " + source);
		}
		else {
//			Debug.Log ("MainMenu: Got 'isOver=false' OnHover event from: " + source);
		}
	}
	

	IEnumerator ReloadCurrentPanelButtons() {
		yield return new WaitForEndOfFrame();
		currentPanelButtons = uiRoot.gameObject.GetComponentsInChildren<UIButton>();
		selectedButtonIndex = 0;
		GetSelectedButton().SendMessage("OnHover", true);

		Array.Sort(
			currentPanelButtons, 
            (UIButton lhs, UIButton rhs) => {
				return lhs.name.CompareTo(rhs.name);
			}
		);
	}


	public void VehicleScreenSelectNextButton() {
		bool releasedUp = InputManager.ActiveDevice.DPadUp.WasReleased;
		bool releasedDown = InputManager.ActiveDevice.DPadDown.WasReleased;
		bool releasedLeft = InputManager.ActiveDevice.DPadLeft.WasReleased;
		bool releasedRight = InputManager.ActiveDevice.DPadRight.WasReleased;


		/* 
			Button index 0 -> top left
			Button index 1 -> top right
			Button index 2 -> bottom left
			Button index 3 -> bottom right
			Button index 4 -> back button
		 */ 
		switch (selectedButtonIndex) {
			// Button index 0 -> top left
			case 0:
				if (releasedUp) {
					selectedButtonIndex = 4;
				}
				else if (releasedDown) {
					selectedButtonIndex = 2;	
				}
				else if (releasedLeft) {
					selectedButtonIndex = 1;		
				}
				else if (releasedRight) {
					selectedButtonIndex = 1;
				}
				break;
		
			// Button index 1 -> top right
			case 1:
				if (releasedUp) {
					selectedButtonIndex = 3;
				}
				else if (releasedDown) {
					selectedButtonIndex = 3;
				}
				else if (releasedLeft) {
					selectedButtonIndex = 0;
				}
				else if (releasedRight) {
					selectedButtonIndex = 0;
				}
				break;
		
			// Button index 2 -> bottom left
			case 2:
				if (releasedUp) {
					selectedButtonIndex = 0;
				}
				else if (releasedDown) {
					selectedButtonIndex = 4;
				}
				else if (releasedLeft) {
					selectedButtonIndex = 3;
				}
				else if (releasedRight) {
					selectedButtonIndex = 3;
				}
				break;
		
			// Button index 3 -> bottom right
			case 3:
				if (releasedUp) {
					selectedButtonIndex = 1;
				}
				else if (releasedDown) {
					selectedButtonIndex = 1;
				}
				else if (releasedLeft) {
					selectedButtonIndex = 2;
				}
				else if (releasedRight) {
					selectedButtonIndex = 2;
				}
				break;

			// Button index 4 -> back button
			case 4:
				if (releasedUp) {
					selectedButtonIndex = 2;
				}
				else if (releasedDown) {
					selectedButtonIndex = 0;
				}
				break;
		
			default:
				break;
		}
	}


	public void SelectNextButton() {
		++selectedButtonIndex;
		if (selectedButtonIndex >= currentPanelButtons.Length) {
			selectedButtonIndex = 0;
		}
	}


	public void SelectPreviousButton() {
		--selectedButtonIndex;
		if (selectedButtonIndex < 0) {
			selectedButtonIndex = currentPanelButtons.Length-1;
		}
	}


	public void SetSelectedButton(UIButton button) {
		Debug.Log ("Button to select: " + button);
		for (int i = 0; i < currentPanelButtons.Length; ++i) {
			if (currentPanelButtons[i] == button) {
				selectedButtonIndex = i;
				return;
			}
		}
		Debug.LogError("Couldn't set '" + button + "' as selected because it isn't active in current UIPanel!");
	}


	public UIButton GetSelectedButton() {
		return currentPanelButtons[selectedButtonIndex];
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
		
		if (serverStarted) {
			
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
		// Credits -> Mode Select
		else if (creditsPanel.activeInHierarchy) {
			OnModeSelectClick();
		}

		StartCoroutine(ReloadCurrentPanelButtons());
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


	public void OnCreditsClick() {
		Debug.Log("Credits Clicked");
		
		HideAllMenus();
		creditsPanel.SetActive(true);
	}
	
	
	public void OnStartServerClick() {
//		HideBackButton();
		
		NetworkManager.StartServer();
		joinServerButton.isEnabled = false;
		
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		
		launchButton.isEnabled = true;
		serverStarted = true;
		
		Debug.Log("Server started status: " + serverStarted);
		
		launchText.text = "LAUNCH";
	}
	
	
	public void OnJoinServerClick() {
//		HideBackButton();
		this.gameObject.AddComponent<NetworkView>();
		
		refreshButton.isEnabled = true;
		
		NetworkManager.RefreshHostList();
		refreshClicked = true;
		
		launchButton.isEnabled = false;
		
		HideAllMenus();
		joinServerPanel.SetActive(true);
		
		launchText.text = "WAITING ON HOST";
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


	public void OnVehicle4Click() {
		LevelManager.SetSpaceship(vehicle4Filepath);
		
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
		
		//if(Network.isClient) {
			//networkView.RPC("PlayerReady", RPCMode.Server);
		//}
	}
	
	
	public void OnLaunchClick() {
		//if(playersReady == NetworkManager.numPlayers) {		//This is assuming no client drops which is not robust
			if(Network.isServer) {							
				MasterServer.UnregisterHost();
			}
			this.gameObject.AddComponent<NetworkView>();
			networkView.RPC("SwitchLoad", RPCMode.All);
			networkView.RPC("LevelLoader", RPCMode.All);
		//}
	}
	
	
	[RPC]
	private void SwitchLoad() {
		lobbyPanel.SetActive(false);
	}
	
	
	[RPC]
	private void LevelLoader() {
		LevelManager.NetworkLoadLevel(level1Filename, 1);	
	}
	
	[RPC]
	private void PlayerReady() {
		playersReady++;
	}
	
	/*	In order to decrement proper we need to keep track of
		playerid which changes each time a player connects
		and that adds another layer of consistency checking.
		This method cannot decrement properly if a player who
		declares himself as ready leaves which causes issues when
		the server is then able to launch without a player being
		ready. Ideally this case works when no one drops.
	*/
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}