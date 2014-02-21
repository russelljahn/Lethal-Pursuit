// Basic, shows simple coloring/texturing and simple cel-shading with the default Lambert lighting model.
// Also shows using a function to process final fragment color.

Shader "BadassVFX/Cel + Boring Lambert Lighting" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
	}

	SubShader {
		
		Tags { "RenderType" = "Opaque" }
		Name "CEL_LAMBERT"
	
		CGPROGRAM
		
		#pragma exclude_renderers xbox360
		#pragma surface surf Lambert finalcolor:final
		
		float4 _MainColor;
		float _ColorMerge;
		float _NumColors;
		

		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		float _OutlineWidth;
		

		void surf (Input IN, inout SurfaceOutput o) {
			float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = texColor;
			o.Alpha = texColor.a;
		 }	
		
		
		void final (Input IN, SurfaceOutput o, inout fixed4 color) {
			color = _MainColor*floor(color * _NumColors)/_NumColors;
		}

		ENDCG
	
	}
	
	
	Fallback "Diffuse"
}