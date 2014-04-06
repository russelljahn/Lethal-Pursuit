using UnityEngine;
using System.Collections;

public class HudEnemyIndicator : MonoBehaviour {

	public Spaceship spaceship;
	public SpaceshipHealth health;

	public UI2DSprite [] indicatorSprites;
	public Camera uiCamera;

	public Sprite onScreenSprite;
	public Sprite offScreenSprite;

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

			GameObject [] spaceships = GameObject.FindGameObjectsWithTag("Spaceship");
			Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0.0f)*0.5f;
			int j = 0;

			for (int i = 0; i < spaceships.Length; ++i) {
				Spaceship ship = spaceships[i].GetComponent<Spaceship>();
				if (ship == spaceship) {
					continue;
				}
				Vector3 screenPosition = spaceship.spaceshipCamera.WorldToScreenPoint(ship.transform.position);

				if (screenPosition.z > 0 && 
				    screenPosition.x > 0 && screenPosition.x < Screen.width &&
				    screenPosition.y > 0 && screenPosition.y < Screen.height) {

					Vector3 newIndicatorPosition = 2.0f*(screenPosition-screenCenter);
					newIndicatorPosition.z = 0f;
					indicatorSprites[j].sprite2D = onScreenSprite;
					indicatorSprites[j].enabled = true;
					indicatorSprites[j].transform.localPosition = newIndicatorPosition;
				}
//				else if (screenPosition.z < 0) {
//					screenPosition.z *= -1.0f;
//					float border = 0.9f;
//					
//					screenPosition -= screenCenter; // Map screen coords origin to center of screen
//					float slope = screenPosition.x/screenPosition.y;
//					//y = mx
//					//x = y/m
//
//					screenPosition.x = Mathf.Clamp((0.5f*Screen.height)/slope, -border, border);
//					screenPosition.y = Mathf.Clamp(slope*(0.5f*Screen.width), -border, border);
//					
//
//					screenPosition += screenCenter;
////					Debug.Log(string.Format("Viewport position for {0}: {1}", ship, viewportPosition));
////					screenPosition = spaceship.spaceshipCamera.ViewportToScreenPoint(viewportPosition);
////					Vector3 newIndicatorPosition = 2.0f*(screenPosition-screenCenter);
//					Vector3 newIndicatorPosition = screenPosition;
//					indicatorSprites[j].sprite2D = offScreenSprite;
//					indicatorSprites[j].enabled = true;
//					indicatorSprites[j].transform.localPosition = newIndicatorPosition;
//				}

				++j;
			}

		}
	}
}
