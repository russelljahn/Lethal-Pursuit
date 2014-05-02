using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Splash : MonoBehaviour {

	public UIPanel logoPanel;
	public UIPanel controllerWarningPanel;
	
	public float fadeRate = 1.0f;
	public AudioClip logoSoundClip;


	// Use this for initialization
	void Start () {
		logoPanel.alpha = 0.0f;
		controllerWarningPanel.alpha = 0.0f;
		
		StartCoroutine(ShowSplash());
	}

	
	IEnumerator ShowSplash() {
		yield return new WaitForSeconds(0.5f);
		while (logoPanel.alpha < 1.0f) {
			logoPanel.alpha = Mathf.Min(1.0f, logoPanel.alpha + fadeRate*Time.deltaTime);
			yield return null;
		}

		audio.PlayOneShot(logoSoundClip);
		yield return new WaitForSeconds(1.35f);

		while (logoPanel.alpha > 0.0f) {
			logoPanel.alpha = Mathf.Max(0.0f, logoPanel.alpha - fadeRate*Time.deltaTime);
			yield return null;
		}

		StartCoroutine(ShowControllerWarning());
	}


	IEnumerator ShowControllerWarning() {
		yield return new WaitForSeconds(0.5f);
		while (controllerWarningPanel.alpha < 1.0f) {
			controllerWarningPanel.alpha = Mathf.Min(1.0f, controllerWarningPanel.alpha + fadeRate*Time.deltaTime);
			yield return null;
		}

		yield return new WaitForSeconds(1.35f);
		
		while (controllerWarningPanel.alpha > 0.0f) {
			controllerWarningPanel.alpha = Mathf.Max(0.0f, controllerWarningPanel.alpha - fadeRate*Time.deltaTime);
			yield return null;
		}
		
		LevelManager.LoadMainMenu(false);
	}
}
