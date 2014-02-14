#pragma strict
import IndieEffects;

/*
----------Billboard script----------
attach this to a GameObject with a mesh renderer to display what the camera is seeing
*/
function Start () {

}

var board : Renderer;
function Update () {
board.material.mainTexture = renderTexture;
}