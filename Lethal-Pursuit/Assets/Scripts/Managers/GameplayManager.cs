using UnityEngine;
using InControl;
using System.Collections;


/* 
 * GameplayManager is a singleton class responsible for general gameplay upkeep, such as 
 * updating the controller inputs.
 */
public class GameplayManager : MonoBehaviour {


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




	void Start () {
		InputManager.Setup();

		/* Register custom input profiles for keyboard button mappings. */
		InputManager.AttachDevice( new UnityInputDevice( new SpaceshipKeyboardProfile1() ) );
		InputManager.AttachDevice( new UnityInputDevice( new SpaceshipKeyboardProfile2() ) );
		
		
	}




	void Update () {
		InputManager.Update();
	}




}
