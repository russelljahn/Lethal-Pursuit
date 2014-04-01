// Basic, shows simple coloring/texturing with the default Lambert lighting model.

Shader "BadassVFX/Boring Lambert" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader {
		
		Tags { "RenderType" = "Opaque" }
		Name "BORING_LAMBERT"
	
		CGPROGRAM
		
		#pragma surface surf Lambert
		
		float4 _MainColor;
		sampler2D _MainTex;
		
		
//		struct Input {
//			float3 viewDir
//			float4 name : COLOR (COLOR is a semantic tag saying that you'll put output color into this variable, used by later shader stages. 
//			float4 screenPos
//			float3 worldPos
//			float3 worldRefl
//			float3 worldNormal
//			float3 worldRefl
//			float3 worldNormal
//		};
		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
		};
		
		
//		struct SurfaceOutput {
//		    half3 Albedo; // Diffuse amount
//		    half3 Normal;
//		    half3 Emission;
//		    half Specular;
//		    half Gloss;
//		    half Alpha;
//		};
		void surf (Input IN, inout SurfaceOutput o) {
			float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = texColor*_MainColor;
			o.Alpha = texColor.a;
		 }

		ENDCG
	
	}
	
	
	Fallback "Diffuse"
}