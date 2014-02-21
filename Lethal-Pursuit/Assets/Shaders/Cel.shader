// Shows simple coloring/texturing and simple cel-shading with a custom lighting function.

Shader "BadassVFX/Cel" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
		_Brightness ("Brightness Factor", Range(1, 10)) = 1
		
	}

	SubShader {
		
		Tags { "RenderType" = "Opaque" }
		Name "CEL_BASE"
	
		CGPROGRAM
		
		#pragma exclude_renderers xbox360
		#pragma surface surf Celshaded finalcolor:final
		
		float4 _MainColor;
		float _ColorMerge;
		float _NumColors;
		float _Brightness;

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
		 
		 
		half4 LightingCelshaded (SurfaceOutput o, half3 lightDir, half attenuation) {
			half4 lightColor;
			half diffuseAmount = dot(o.Normal, lightDir);
			diffuseAmount = floor(diffuseAmount * _NumColors)/_NumColors;
			lightColor.rgb = o.Albedo * _LightColor0.rgb * diffuseAmount * attenuation * 2;
			lightColor.a = o.Alpha;
			return lightColor;
		}
		
		
		
		void final (Input IN, SurfaceOutput o, inout fixed4 color) {
			color = _MainColor*floor(color * _NumColors)/_NumColors * _Brightness;
		}

		ENDCG
	
	}
	
	
	Fallback "Diffuse"
}