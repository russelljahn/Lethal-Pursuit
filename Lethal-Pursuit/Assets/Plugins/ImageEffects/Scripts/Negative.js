#pragma strict
/*

				----------Negative----------
When i was playing around with the indie effects motion blur shader, i got 
this effect by accident. enjoy!
*/
@script RequireComponent (IndieEffects)
@script AddComponentMenu ("Indie Effects/Negative")
import IndieEffects;

private var ThermoMat : Material;
var shader : Shader;
var noise : float;

function Start () {
	ThermoMat = new Material(shader);
}

function Update () {
	ThermoMat.SetFloat("_Noise",noise);
	ThermoMat.SetTexture("_MainTex", renderTexture);
}

function OnPostRender () {
	FullScreenQuad(ThermoMat);
}