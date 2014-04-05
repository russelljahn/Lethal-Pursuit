using UnityEngine;
using System.Collections;

public class HudEnemyIndicator : MonoBehaviour {

	public Spaceship spaceship;
	public SpaceshipHealth health;

	public UI2DSprite [] indicatorSprites;
	public Camera uiCamera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (spaceship == null) {
			Spaceship ship = GameplayManager.spaceship;
			if (ship != null) {
				spaceship = ship.GetComponent<Spaceship>();
				health = ship.GetComponent<SpaceshipHealth>();
			}
		}
		else {
			for (int i = 0; i < indicatorSprites.Length; ++i) {
				indicatorSprites[i].enabled = false;
			}

			Spaceship [] spaceships = GameObject.FindObjectsOfType<Spaceship>();
			int j = 0;
			for (int i = 0; i < spaceships.Length; ++i) {
				Spaceship ship = spaceships[i];
				if (ship == spaceship) {
					continue;
				}
				Vector3 screenPosition = spaceship.spaceshipCamera.WorldToScreenPoint(ship.transform.position);
				Debug.Log (string.Format("Screen position of {0}: {1}", ship, screenPosition));
				Debug.Log ("screenCenter: " + new Vector3(Screen.width, Screen.height, 0.0f)*0.5f);
				if (screenPosition.z > 0 && 
				    screenPosition.x > 0 && screenPosition.x < Screen.width &&
				    screenPosition.y > 0 && screenPosition.y < Screen.height) {
					Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0.0f)*0.5f;
					Vector3 newIndicatorPosition = 2.0f*(screenPosition-screenCenter);
					newIndicatorPosition.z = ship.transform.position.z;
					indicatorSprites[j].enabled = true;
					indicatorSprites[j].transform.localPosition = newIndicatorPosition;
				}
//				else if (screenPosition.z < 0) {
//					screenPosition.z *= -1.0f;
//				}

				++j;
			}

		}
	}
}
