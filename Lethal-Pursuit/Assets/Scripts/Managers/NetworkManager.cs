using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	private string gameType = "CS354T-Galacticats-LP";
	public string gameName = "Lethal-Pursuit: Krunklicious edition";
	private string gameComment = "Network Test Run";
	
	public GameObject player;
	public Transform spawnPoint;
	public string nameOfSpaceship = "Spaceships/Spaceship02";
	
	private HostData[] hostData;
	private bool refreshing = false;
	
	//GUI Params
	private float buttonX;
	private float buttonY;
	private float buttonW;
	private float buttonH;
	
	// Use this for initialization
	void Start () 
	{
		buttonX = Screen.width * 0.05f;
		buttonY = Screen.height * 0.05f;
		buttonW = Screen.width * 0.1f;
		buttonH = Screen.width * 0.05f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(refreshing)
		{
			if(MasterServer.PollHostList().Length > 0)
			{
				refreshing = false;
				Debug.Log ("Number of available games: " + MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
			
		}
	}
	
	void OnGUI ()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(buttonX,
			                        buttonY,
			                        buttonW,
			                        buttonH),
			               "Start Server"))
			{
				StartServer ();
			}
			
			if (GUI.Button(new Rect(buttonX, 
			                        buttonY * 1.2f + buttonH, 
			                        buttonW,
			                        buttonH),
			               "Refresh"))
			{
				RefreshHostList ();
			}
			
			if(hostData != null)
			{
				for(int i=0; i<hostData.Length; i++)
				{
					if(GUI.Button(new Rect(buttonX * 1.2f + buttonW,
					                       buttonY * 1.2f + (buttonH * i),
					                       buttonW * 3, buttonH),
					              hostData[i].gameName))
					{
						Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
	
	void StartServer ()
	{
		Debug.Log ("Starting server.........");
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost (gameType, gameName, gameComment);
	}
	
	void OnServerInitialized ()
	{
		Debug.Log ("Server ready");
		SpawnPlayer();
	}
	
	void OnMasterServerEvent (MasterServerEvent msevent)
	{
		Debug.Log ("Received MSE");
		if (msevent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("MSE Registration Suceeded");
		}
	}
	
	void RefreshHostList ()
	{
		Debug.Log ("Refreshing server list.........");
		MasterServer.RequestHostList (gameType);
		refreshing = true;
	}
	
	
	void OnConnectedToServer ()
	{
		Debug.Log ("Connected to server");
		SpawnPlayer();
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
		
		//Need to have a level loaded back to menu here
	}
	
	void SpawnPlayer()
	{
		GameObject spaceship = Network.Instantiate(
			Resources.Load (nameOfSpaceship),
			spawnPoint.position, 
			Quaternion.identity,
			0) as GameObject;
	
		SpaceshipCamera cam = GameObject.FindWithTag("MainCamera").GetComponent<SpaceshipCamera>();
		cam.SetSpaceship(spaceship.GetComponent<Spaceship>());

	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}