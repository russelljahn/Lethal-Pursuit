using UnityEngine;
using System.Collections;


public class LevelTutorial : Level {

	public LevelTutorial() {
		name = "Training Mode";
		description = "Training course introducing you to the game.";

		sceneName = "Tutorial";
		
		maximumPlayers = 1;
		lapsToWin = 3;
	}

};
