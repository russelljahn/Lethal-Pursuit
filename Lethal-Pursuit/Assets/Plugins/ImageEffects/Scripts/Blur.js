#pragma strict

@script RequireComponent (IndieEffects)
@script AddComponentMenu ("Indie Effects/Blur")
import IndieEffects;

private var blurMat : Material;
var blurShader : Shader;
@range(0,5)
var blur : float;

function Start () {
	blurMat = new Material(blurShader);
}

function Update () {
	blurMat.SetTexture("_MainTex", renderTexture);
	blurMat.SetFloat("_Threshold", 0);
	blurMat.SetFloat("_Amount", blur);
}

function OnPostRender () {
	FullScreenQuad(blurMat);
}