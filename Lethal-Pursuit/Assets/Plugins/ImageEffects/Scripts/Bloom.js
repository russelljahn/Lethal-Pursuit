#pragma strict

@script RequireComponent (IndieEffects)
@script AddComponentMenu ("Indie Effects/Bloom")
import IndieEffects;

private var bloomMat : Material;
var bloomShader : Shader;
@range(0,2)
var amount : float;

function Start () {
	bloomMat = new Material(bloomShader);
}

function Update () {
	bloomMat.SetTexture("_MainTex", renderTexture);
	bloomMat.SetTexture("_BlurTex", renderTexture);
	bloomMat.SetFloat("_Threshold", 0);
	bloomMat.SetFloat("_Amount", amount);
}

function OnPostRender () {
	FullScreenQuad(bloomMat);
}