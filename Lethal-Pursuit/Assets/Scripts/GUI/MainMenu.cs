using UnityEngine;
using System;
using System.Collections;
using InControl;

public class MainMenu : MonoBehaviour {
	
	public GameObject titlePanel;
	public GameObject backPanel;
	public GameObject vehicleSelectPanel;
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
	public UILabel gameNameText;
	
	
	public  GameObject[] serverButtons;
	public  UILabel[] 	 buttonLabels;
	private HostData[]   hostdata;
	
	private bool refreshClicked = false;
	private static int lastLevelPrefix;
	
	private bool serverStarted = false;
	
	private bool tutorial = false;
	private bool client = false;
	
	public string vehicle1Filepath = "Spaceships/Dauntless";
	public string vehicle2Filepath = "Spaceships/Helldiver";
	public string vehicle3Filepath = "Spaceships/Little John";
	public string vehicle4Filepath = "Spaceships/Mufasa";
	
	public string tutorialFilename = "Tutorial";
	public string level1Filename = "Arena";
	
	private int playersReady = 1;
	public UIRoot uiRoot;
	
	public Sprite buttonNormalSprite;
	public Sprite buttonHoverSprite;

	public float buttonNormalOpacity = 0.6f;
	public float buttonTweenDuration = 0.04f;

	public GameObject vehicleSelectFrame;

	public UI2DSprite vehicleSelectShip;
	public UI2DSprite vehicleSelectHealthStat;
	public UI2DSprite vehicleSelectSpeedStat;
	public UI2DSprite vehicleSelectEmblem;
	public UILabel vehicleSelectName;

	public Sprite dauntlessSprite;
	public Sprite helldiverSprite;
	public Sprite littleJohnSprite;
	public Sprite mufasaSprite;
	
	public Sprite dauntlessEmblem;
	public Sprite helldiverEmblem;
	public Sprite littleJohnEmblem;
	public Sprite mufasaEmblem;

	public Color dauntlessEmblemColor;
	public Color helldiverEmblemColor;
	public Color littleJohnEmblemColor;
	public Color mufasaEmblemColor;

	public UILabel multiplayerHubText;
	public GameObject multiplayerHubFrame;
	
	
	


//	public Color buttonNormalColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
//	public Color buttonHoverColor = new Color(0.07059f, 0.8826f, 0.8471f);

	public Color buttonNormalTextColor = Color.black;
	public Color buttonHoverTextColor = new Color(0.4784f, 0.5373f, 0.8471f);

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
			GetSelectedButton().GetComponent<UIWidget>().color = GetSelectedButton().pressed;
			GetSelectedButton().SendMessage("OnPress", true);
		}
		else if (releasedConfirm) {
			GetSelectedButton().SendMessage("OnClick");
		}
		else if (releasedCancel) {
			OnClickBack();
			StartCoroutine(ReloadCurrentPanelButtons());
		}
		else if (releasedDown || releasedRight) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectNextButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}
		else if (releasedUp || releasedLeft) {
			GetSelectedButton().SendMessage("OnHover", false);
			SelectPreviousButton();
			GetSelectedButton().SendMessage("OnHover", true);
		}

		if (releasedUp || releasedDown || releasedLeft || releasedRight) {
			if (vehicleSelectPanel.activeInHierarchy) {
				UpdateVehicleSelectScreen();
			}
			else if (multiplayerHubPanel.activeInHierarchy) {
				UpdateMultiplayerHubScreen();
			}
		}
	}


	// Make this script subscribe to ui events from buttons in the scene
	void RegisterEventHandlers() {
//		uiRoot = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<UIRoot>();
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
		UIButton button = source.GetComponentInChildren<UIButton>();
		UILabel text = source.transform.parent.GetComponentInChildren<UILabel>();

		if (isOver) {
			SetSelectedButton(button);
			button.GetComponent<UI2DSprite>().sprite2D = buttonHoverSprite;
			text.color = buttonHoverTextColor;
//			Debug.Log ("MainMenu: Got 'isOver=true' OnHover event from: " + source);
			if (vehicleSelectPanel.activeInHierarchy) {
				UpdateVehicleSelectScreen();
			}
			else if (multiplayerHubPanel.activeInHierarchy) {
				UpdateMultiplayerHubScreen();
			}
		}
		else {
			button.GetComponent<UI2DSprite>().sprite2D = buttonNormalSprite;
			text.color = buttonNormalTextColor;
				
//			Debug.Log ("MainMenu: Got 'isOver=false' OnHover event from: " + source);
		}


	}
	

	IEnumerator ReloadCurrentPanelButtons() {

		yield return new WaitForEndOfFrame();
		currentPanelButtons = uiRoot.gameObject.GetComponentsInChildren<UIButton>();

		Array.Sort(
			currentPanelButtons, 
            (UIButton lhs, UIButton rhs) => {
				return lhs.name.CompareTo(rhs.name);
			}
		);

		for (int i = 0; i < currentPanelButtons.Length; ++i) {
			UIButton button = currentPanelButtons[i];
			button.defaultColor = new Color(1.0f, 1.0f, 1.0f, buttonNormalOpacity);
			button.GetComponent<UI2DSprite>().sprite2D = buttonNormalSprite;
			button.transform.parent.GetComponentInChildren<UILabel>().color = buttonNormalTextColor;
		}
		
		selectedButtonIndex = 0;
		GetSelectedButton().SendMessage("OnHover", true);
		
	}


	public void UpdateMultiplayerHubScreen() {
		
		/* 
			Button index 0 -> Create Game
			Button index 1 -> Join Game
			Button index 2 -> Back button
		 */ 
		switch (selectedButtonIndex) {
			// Button index 0 -> Create Game
			case 0:
				multiplayerHubText.text = "Mode:   Deathmatch\nTarget: 10 Kills\nArena:  Abandoned Missile Silo\nHost a game of multiplayer combat with up to 3 others.";
				break;

			// Button index 1 -> Join Game
			case 1:
				multiplayerHubText.text = "Mode:   Deathmatch\nTarget: 10 Kills\nArena:  Abandoned Missile Silo\nJoin a game of multiplayer combat with up to 3 others.";
				break;

			// Button index 2 -> Back button
			case 2:
				break;

			default:
				break;
		}
	}


	public void UpdateVehicleSelectScreen() {

		/* 
			Button index 0 -> Dauntless
			Button index 1 -> Helldiver
			Button index 2 -> Little John
			Button index 3 -> Mufasa
			Button index 4 -> Back button
		 */ 
		switch (selectedButtonIndex) {
			// Button index 0 -> Dauntless
			case 0:
				vehicleSelectShip.sprite2D = dauntlessSprite;
				vehicleSelectName.text = "Dauntless";
				vehicleSelectHealthStat.transform.localScale = new Vector3(0.45f, 1.0f, 1.0f);
				vehicleSelectSpeedStat.transform.localScale = new Vector3(0.55f, 1.0f, 1.0f);
				vehicleSelectEmblem.sprite2D = dauntlessEmblem;
				vehicleSelectEmblem.color = dauntlessEmblemColor;
				break;
		
			// Button index 1 -> Helldiver
			case 1:
				vehicleSelectShip.sprite2D = helldiverSprite;
				vehicleSelectName.text = "Helldiver";
				vehicleSelectHealthStat.transform.localScale = new Vector3(0.55f, 1.0f, 1.0f);
				vehicleSelectSpeedStat.transform.localScale = new Vector3(0.45f, 1.0f, 1.0f);
				vehicleSelectEmblem.sprite2D = helldiverEmblem;
				vehicleSelectEmblem.color = helldiverEmblemColor;
				break;
		
			// Button index 2 -> Little John
			case 2:
				vehicleSelectShip.sprite2D = littleJohnSprite;
				vehicleSelectName.text = "Little John";
				vehicleSelectHealthStat.transform.localScale = new Vector3(0.3f, 1.0f, 1.0f);
				vehicleSelectSpeedStat.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				vehicleSelectEmblem.sprite2D = littleJohnEmblem;
				vehicleSelectEmblem.color = littleJohnEmblemColor;
				break;
		
			// Button index 3 -> Mufasa
			case 3:
				vehicleSelectShip.sprite2D = mufasaSprite;
				vehicleSelectName.text = "Mufasa";
				vehicleSelectHealthStat.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				vehicleSelectSpeedStat.transform.localScale = new Vector3(0.3f, 1.0f, 1.0f);
				vehicleSelectEmblem.sprite2D = mufasaEmblem;
				vehicleSelectEmblem.color = mufasaEmblemColor;
				break;

			// Button index 4 -> Back button
			case 4:
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
		vehicleSelectPanel.SetActive(false);
		multiplayerHubPanel.SetActive(false);
		lobbyPanel.SetActive(false);
		joinServerPanel.SetActive(false);
		creditsPanel.SetActive(false);
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

		if (networkView != null) {
			Destroy(this.networkView);
		}
		
		// Exit
		if (titlePanel.activeInHierarchy && currentPanelButtons.Length > 0) {
			GetSelectedButton().SendMessage("OnHover", false);
//			selectedButtonIndex = currentPanelButtons.Length-1;
		}
		// Vehicle Select -> MultiplayerHub
		if (vehicleSelectPanel.activeInHierarchy) {
			LevelManager.LoadMainMenu(false);
//			OnMultiplayerClick();
		}
		// MultiplayerHub -> Mode Select
		else if (multiplayerHubPanel.activeInHierarchy) {
			OnTitleClick();
		}
		// Lobby -> MultiplayerHub (if Multiplayer CreateServer)/JoinServer (if Multiplayer JoinServer)
		else if (lobbyPanel.activeInHierarchy) {
			LevelManager.LoadMainMenu(false);
		}
		// JoinServer -> MultiplayerHub
		else if (joinServerPanel.activeInHierarchy) {
			LevelManager.LoadMainMenu(false);
//			OnMultiplayerClick();
		}
		// Credits -> Mode Select
		else if (creditsPanel.activeInHierarchy) {
			OnTitleClick();
		}

	}
	
	
	public void OnExitClick() {
		Debug.Log("Exit Clicked");
		LevelManager.Quit();
	}


	public void OnTitleClick() {
		Debug.Log("Title Clicked");
		HideAllMenus();
		titlePanel.SetActive(true);
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
		UpdateVehicleSelectScreen();
	}
	
	
	public void OnTutorialClick() {
		Debug.Log("Tutorial Clicked");
		NetworkManager.SetSinglePlayer(true);
		tutorial = true;
		
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		UpdateVehicleSelectScreen();
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
		UpdateVehicleSelectScreen();
		
		launchButton.isEnabled = true;
		serverStarted = true;
		
		Debug.Log("Server started status: " + serverStarted);
		
		launchText.text = "Launch";
		gameNameText.text = "Hosting " + NetworkManager.gameName;
	}
	
	
	public void OnJoinServerClick() {
//		HideBackButton();
		refreshButton.isEnabled = true;
		
		NetworkManager.RefreshHostList();
		refreshClicked = true;

		launchButton.enabled = false;
		
		HideAllMenus();
		joinServerPanel.SetActive(true);

		launchText.text = "Waiting...";
		launchText.color = buttonNormalTextColor;
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
		UpdateVehicleSelectScreen();
	}
	
	
	public void OnServer2Click() {
		NetworkManager.JoinServer(1);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		UpdateVehicleSelectScreen();
	}
	
	
	public void OnServer3Click() {
		NetworkManager.JoinServer(2);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		UpdateVehicleSelectScreen();
	}
	
	
	public void OnServer4Click() {
		NetworkManager.JoinServer(3);
		HideAllMenus();
		vehicleSelectPanel.SetActive(true);
		UpdateVehicleSelectScreen();
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
		if (networkView == null) {
			this.gameObject.AddComponent<NetworkView>();
		}
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