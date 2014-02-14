#pragma strict
/*
----------Indie Effects Base----------
This is the base for all other image effects to occur. this also incorporates motion blur as a
built-in feature for those who are impatient to see motion blur on unity free!
*/

@script RequireComponent(Camera);
@script AddComponentMenu("Indie Effects/base(with motion blur)");

static var renderTexture : Texture2D;
static var depthTexture : Texture3D;
private var BlurMat : Material;
static var mainCam : Camera;
var motionBlur : boolean;
var BlurShader : Shader;
var Accumulation : float = 0.65;

static function FullScreenQuad(renderMat : Material){
	GL.PushMatrix();
	for (var i = 0; i < renderMat.passCount; ++i) {
		renderMat.SetPass(i);
		GL.LoadOrtho();
		GL.Begin(GL.QUADS); // Quad
		GL.Color(Color(1,1,1,1));
		GL.MultiTexCoord(0,Vector3(0,0,0));
		GL.Vertex3(0,0,0);
		GL.MultiTexCoord(0,Vector3(0,1,0));
		GL.Vertex3(0,1,0);
		GL.MultiTexCoord(0,Vector3(1,1,0));
		GL.Vertex3(1,1,0);
		GL.MultiTexCoord(0,Vector3(1,0,0));
		GL.Vertex3(1,0,0);
		GL.End();
	}
	GL.PopMatrix();
}

function Start () {
	renderTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
	BlurMat = new Material (BlurShader);
}

function Update () {
	BlurMat.SetTexture("_MainTex",renderTexture);
	BlurMat.SetFloat("_FrameAccumulation",Accumulation);
}

function OnPostRender() {
	if (motionBlur == true){
	FullScreenQuad(BlurMat);
	}
	renderTexture.Resize(Screen.width, Screen.height, TextureFormat.RGB24, false);
	renderTexture.ReadPixels(Rect(0,0,Screen.width,Screen.height), 0, 0);
	renderTexture.Apply();
}