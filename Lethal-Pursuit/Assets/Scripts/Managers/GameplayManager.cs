using UnityEngine;
using InControl;
using System.Collections;
using System;


/* 
 * GameplayManager is a singleton class responsible for general gameplay upkeep, such as 
 * updating the controller inputs.
 */
public class GameplayManager : MonoBehaviour {

	private static bool alreadyAwoken = false;
	public static bool keyboardControlsActive = true;

	public static Spaceship spaceship {
		get {
			return GameplayManager.GetLocalSpaceship();
		}
		set {

		}
	}

	private static GameplayManager singletonInstance = null;


	public static GameplayManager instance {
		get {
			/* If first time accessing instance, then find it... */
			if (singletonInstance == null) {
				singletonInstance = FindObjectOfType(typeof (GameplayManager)) as GameplayManager;
			}
			
			/* If instance is null, then no GameManager exists in the scene, so create one. */
			if (singletonInstance == null) {
				GameObject obj = new GameObject("GameplayManager");
				singletonInstance = obj.AddComponent(typeof (GameplayManager)) as GameplayManager;
				obj.name = "GameplayManager";
			}
			return singletonInstance;
		}
	}

	public static bool invertYAxis = true;




	void Awake () {

		if (alreadyAwoken) {
			return;
		}

		alreadyAwoken = true;
		GameObject.DontDestroyOnLoad(this.gameObject);
		InputManager.InvertYAxis = true;
		GameplayManager.invertYAxis = true;
		InputManager.Setup();
		
		/* Register custom input profiles for keyboard button mappings. */
		InputManager.AttachDevice( new UnityInputDevice( new SpaceshipKeyboardProfile1() ) );
		InputManager.AttachDevice( new UnityInputDevice( new SpaceshipKeyboardProfile2() ) );

		InputManager.OnActiveDeviceChanged += OnActiveDeviceChanged;
		InputManager.OnDeviceAttached += OnDeviceAttached;
		InputManager.OnDeviceDetached += OnDeviceDetached;

		UpdateKeyboardControlsActive(InputManager.ActiveDevice);
	}




	void Update () {
		InputManager.Update();
	}



	void OnDeviceAttached(InputDevice inputDevice) {
		Debug.Log( "Attached: " + inputDevice.Name );
		UpdateKeyboardControlsActive(inputDevice);
	}



	void OnDeviceDetached(InputDevice inputDevice) {
		Debug.Log( "Detached: " + inputDevice.Name );
		UpdateKeyboardControlsActive(inputDevice);
	}



	void OnActiveDeviceChanged(InputDevice inputDevice) {
		Debug.Log( "Switched: " + inputDevice.Name );
		UpdateKeyboardControlsActive(inputDevice);
	}


	private static void UpdateKeyboardControlsActive(InputDevice inputDevice) {
		if (inputDevice.Name.Equals("Keyboard")) {
			keyboardControlsActive = true;
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
		else {
			keyboardControlsActive = false;
			Screen.showCursor = false;
			Screen.lockCursor = true;
		}
	}


	public static Spaceship GetLocalSpaceship() {
		if (NetworkManager.IsSinglePlayer()) {
			GameObject ship = GameObject.FindWithTag("Spaceship");
			return ship.GetComponent<Spaceship>();
		}
		else {
			GameObject[] ships = GameObject.FindGameObjectsWithTag("Spaceship");
			
			for (int i = 0; i < ships.Length; ++i) {
				Debug.Log (String.Format("Looking at ship {0}: {1}", i, ships[i]));

				NetworkView networkView = ships[i].GetComponent<NetworkView>();

				if (networkView == null) {
					throw new Exception("Spaceship missing NetworkView: " + ships[i].name);
				}
				else if (networkView.isMine) {
					Debug.Log (String.Format("Ship as {0} is mine!: {1}", i, ships[i]));
					return ships[i].GetComponent<Spaceship>();
				}
				else {
					Debug.Log (
						String.Format("Ship as {0} IS NOT mine...: {1}, {2}, is mine? ", 
					              i, ships[i], networkView, networkView.isMine.ToString()
					    )
					);
				}
			}		
		}
		return null;
	}


}
