#pragma strict
import IndieEffects;

@script RequireComponent(Camera);
@script AddComponentMenu("Indie Effects/Radial Blur & GodRays");

private var sampleMat : Material;
var GodRaysShader : Shader;
var RadialBlurShader : Shader;
var BlurAmount : float;

enum BlurMode {
	GodRays = 0,
	Radialblur = 1,
}

var BlurMode : BlurMode = 0;
var GodRaysIntensity : float = 0.3;

function RadialBlurQuad1 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.01,-0.01,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.01,1.01,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.01,1.01,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.01,-0.01,0);
		GL.End();
	}
	GL.PopMatrix();
}

function RadialBlurQuad2 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.03,-0.03,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.03,1.03,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.03,1.03,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.03,-0.03,0);
		GL.End();
	}
	GL.PopMatrix();
}

function RadialBlurQuad3 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.05,-0.05,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.05,1.05,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.05,1.05,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.05,-0.05,0);
		GL.End();
	}
	GL.PopMatrix();
}

function RadialBlurQuad4 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.07,-0.07,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.07,1.07,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.07,1.07,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.07,-0.07,0);
		GL.End();
	}
	GL.PopMatrix();
}

function RadialBlurQuad5 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.09,-0.09,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.09,1.09,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.09,1.09,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.09,-0.09,0);
		GL.End();
	}
	GL.PopMatrix();
}

function RadialBlurQuad6 (renderMat : Material) {
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(-0.12,-0.12,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(-0.12,1.12,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1.12,1.12,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1.12,-0.12,0);
		GL.End();
	}
	GL.PopMatrix();
}


function Start () {
	if (BlurMode == 0){
	sampleMat = new Material(GodRaysShader);
	}
	if (BlurMode == 1){
	sampleMat = new Material(RadialBlurShader);
	}
}

function Update () {
	sampleMat.mainTexture = renderTexture;
	if (BlurMode == 0){
		sampleMat.SetFloat("_Intensity",GodRaysIntensity);
	}
	if (BlurMode == 1){
		sampleMat.SetFloat("_Blur",BlurAmount);
	}
}

function OnPostRender () {
	FullScreenQuad(sampleMat);
	RadialBlurQuad1(sampleMat);
	RadialBlurQuad2(sampleMat);
	RadialBlurQuad3(sampleMat);
	RadialBlurQuad4(sampleMat);
	RadialBlurQuad5(sampleMat);
	RadialBlurQuad6(sampleMat);
}