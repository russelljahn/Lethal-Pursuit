using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	private static string gameType = "2P Multiplayer: ";
	public  static string gameName = "Hosted Game";
	private static string gameComment = "Network Test Run";
	
	private static HostData[] hostData;
	private static bool refreshing = false;
	private static bool singlePlayer = true;

	private static NetworkManager singletonInstance;

	private static string chosenShip = null;	
	
	private static NetworkManager instance {
		get {
			// If first time accessing instance, then find it...
			if (singletonInstance == null) {
				singletonInstance = FindObjectOfType(typeof (NetworkManager)) as NetworkManager;
			}
			
			// If instance is null, then no GameManager exists in the scene, so create one.
			if (singletonInstance == null) {
				GameObject obj = new GameObject("NetworkManager");
				singletonInstance = obj.AddComponent(typeof (NetworkManager)) as NetworkManager;
				obj.name = "Network Manager";
				//Debug.Log ("Could not find a LevelManager object, so automatically generated one.");
			}
			
			return singletonInstance;
		}
	}

	public void Awake() {
		DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(refreshing)
		{
			Debug.Log("Refreshing......");
			if(MasterServer.PollHostList().Length > 0)
			{
				refreshing = false;
				Debug.Log ("Number of available games: " + MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
			
		}
	}

	public static void StartServer ()
	{
		Debug.Log ("Starting server.........");
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		Debug.Log ("Registering Host.........");
		MasterServer.RegisterHost (gameType, gameName, gameComment);
	}
	
	void OnServerInitialized ()
	{
		Debug.Log ("Server ready");
		//SpawnPlayer();
	}
	
	void OnMasterServerEvent (MasterServerEvent msevent)
	{
		Debug.Log ("Received MSE");
		if (msevent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("MSE Registration Suceeded");
		}
	}
	
	public static void RefreshHostList ()
	{
		Debug.Log ("Refreshing server list.........");
		MasterServer.RequestHostList (gameType);
		refreshing = true;
	}
	
	
	void OnConnectedToServer ()
	{
		Debug.Log ("Connected to server");
		//SpawnPlayer();
	}
	
	void OnFailedConnection (NetworkConnectionError error)
	{
		Debug.Log ("Failed to connect ot server: " + error.ToString());
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Clean up a bit after server quit");
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		
		LevelManager.LoadLevel("MainMenu");
		//Need to have a level loaded back to menu here
	}

	public static void JoinServer(int serverIndex) {
		Network.Connect(hostData[serverIndex]);
	}
	
	public static bool IsServerListReady() {
		if(hostData != null) {
			return !refreshing && hostData.Length > 0;
		}
		return false;
	}

	public static void SetSinglePlayer(bool mode) {
		singlePlayer = mode;
	}

	public static bool IsSinglePlayer() {
		return singlePlayer;
	}

	public static HostData[] GetHostData() {
		return hostData;
	}

	public static bool IsPlayerVarsSet() {
		return chosenShip != null;
	}
	
	public static void ServerCleanup() {
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}