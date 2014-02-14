#pragma strict

/*
---------- Anti-Aliasing Indie Effects----------

This is an adaption of Unity Pro's AA Script, done by TheBlur (me)

*/
@script RequireComponent (Camera)
@script AddComponentMenu ("Indie Effects/Anti-Aliasing")

enum AAMode {
	FXAA2 = 0,
	FXAA3Console = 1,		
	FXAA1PresetA = 2,
	FXAA1PresetB = 3,
	NFAA = 4,
	SSAA = 5,
	DLAA = 6,
}
	public var mode : AAMode = AAMode.FXAA3Console;

	public var showGeneratedNormals : boolean = false;
	public var offsetScale : float = 0.2;
	public var blurRadius : float = 18.0;

	public var edgeThresholdMin : float = 0.05f;
	public var edgeThreshold : float = 0.2f;
	public var edgeSharpness : float  = 4.0f;
		
	public var dlaaSharp : boolean = false;

	public var ssaaShader : Shader;
	private var ssaa : Material;
	public var dlaaShader : Shader;
	private var dlaa : Material;
	public var nfaaShader : Shader;
	private var nfaa : Material;	
	public var shaderFXAAPreset2 : Shader;
	private var materialFXAAPreset2 : Material;
	public var shaderFXAAPreset3 : Shader;
	private var materialFXAAPreset3 : Material;
	public var shaderFXAAII : Shader;
	private var materialFXAAII : Material;
	public var shaderFXAAIII : Shader;
	private var materialFXAAIII : Material;
		
	function CurrentAAMaterial () : Material
	{
		var returnValue : Material = null;

		switch(mode) {
			case AAMode.FXAA3Console:
				returnValue = materialFXAAIII;
				break;
			case AAMode.FXAA2:
				returnValue = materialFXAAII;
				break;
			case AAMode.FXAA1PresetA:
				returnValue = materialFXAAPreset2;
				break;
			case AAMode.FXAA1PresetB:
				returnValue = materialFXAAPreset3;
				break;
			case AAMode.NFAA:
				returnValue = nfaa;
				break;
			case AAMode.SSAA:
				returnValue = ssaa;
				break;
			case AAMode.DLAA:
				returnValue = dlaa;
				break;	
			default:
				returnValue = null;
				break;
			}
			
		return returnValue;
	}

	function Start () {
			
		materialFXAAPreset2 = new Material (shaderFXAAPreset2);
		materialFXAAPreset3 = new Material (shaderFXAAPreset3);
		materialFXAAII = new Material (shaderFXAAII);
		materialFXAAIII = new Material (shaderFXAAIII);
		nfaa = new Material (nfaaShader);
		ssaa = new Material (ssaaShader); 
		dlaa = new Material (dlaaShader); 	            
	}
	
	function Update () {
	materialFXAAPreset2.mainTexture = IndieEffects.renderTexture;
	materialFXAAPreset3.mainTexture = IndieEffects.renderTexture;
	materialFXAAII.mainTexture = IndieEffects.renderTexture;
	materialFXAAIII.mainTexture = IndieEffects.renderTexture;
	nfaa.mainTexture = IndieEffects.renderTexture;
	ssaa.mainTexture = IndieEffects.renderTexture;
	dlaa.mainTexture = IndieEffects.renderTexture;
	}
	function OnPostRender() {

 		// .............................................................................
		// FXAA antialiasing modes .....................................................
		
		if (mode == AAMode.FXAA3Console && (materialFXAAIII != null)) {
			materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
			materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
			materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);		
		
            IndieEffects.FullScreenQuad(materialFXAAIII);
        }        
		else if (mode == AAMode.FXAA1PresetB && (materialFXAAPreset3 != null)) {
            IndieEffects.FullScreenQuad(materialFXAAPreset3);
        }
        else if(mode == AAMode.FXAA1PresetA && materialFXAAPreset2 != null) {
            IndieEffects.renderTexture.anisoLevel = 4;
            IndieEffects.FullScreenQuad(materialFXAAPreset2);
            IndieEffects.renderTexture.anisoLevel = 0;
        }
        else if(mode == AAMode.FXAA2 && materialFXAAII != null) {
            IndieEffects.FullScreenQuad(materialFXAAII);
        }
		else if (mode == AAMode.SSAA && ssaa != null) {

		// .............................................................................
		// SSAA antialiasing ...........................................................
			
			IndieEffects.FullScreenQuad(ssaa);								
		}
		else if (mode == AAMode.DLAA && dlaa != null) {

		// .............................................................................
		// DLAA antialiasing ...........................................................
		
			IndieEffects.renderTexture.anisoLevel = 0;	
			var interim = IndieEffects.renderTexture;
			IndieEffects.FullScreenQuad(dlaa);			
			IndieEffects.FullScreenQuad(dlaa);					
		}
		else if (mode == AAMode.NFAA && nfaa != null) {

		// .............................................................................
		// nfaa antialiasing ..............................................
			
			IndieEffects.renderTexture.anisoLevel = 0;	
		
			nfaa.SetFloat("_OffsetScale", offsetScale);
			nfaa.SetFloat("_BlurRadius", blurRadius);
				
			IndieEffects.FullScreenQuad(nfaa);					
		}
		else {
			// none of the AA is supported, fallback to a simple blit
			IndieEffects.FullScreenQuad(null);
		}
	}