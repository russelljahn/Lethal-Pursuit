using UnityEngine;
using System.Collections;

/* 
	Class containing info about a level.

 */
public class Level {

	public string name = "Dummy Level Name";
	public string description = "I am a rad level description! :D";

	public string sceneName = "DummySceneName";

	public int maximumPlayers = 1;
	public int lapsToWin = 3;


	public override string ToString() {
		return string.Format ("[Level named '{0}' in scene '{1}']", name, sceneName);
	}
	
		
};
