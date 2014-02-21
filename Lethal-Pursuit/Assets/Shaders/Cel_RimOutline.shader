Shader "BadassVFX/Cel (Rim Outline)" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
		
		_OutlineColor ("Outline Color", Color) = (0.5, 0.5, 0.5, 1)
		_OutlineWidth ("Outline Width", Range(0.1, 1.0)) = 0.4
		_OutlineFalloff ("Outline Falloff", Range(.5, 5)) = 4
		
		_Brightness ("Brightness Factor", Range(1, 10)) = 1
	}

	SubShader {
		
		Tags { "RenderType" = "Opaque" }
		Name "CEL_RIM_OUTLINE"
	
		CGPROGRAM
		
		#pragma exclude_renderers xbox360
		#pragma surface surf Celshaded finalcolor:final
		
		float4 _MainColor;
		float _ColorMerge;
		float _NumColors;
		

		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		float _OutlineWidth;
		float _OutlineFalloff;
		float4 _OutlineColor;
		float _Brightness;
		

		

		void surf (Input IN, inout SurfaceOutput o) {
			// Create outlines on surfaces pointing away from camera
			half edge = saturate(dot (o.Normal, normalize(IN.viewDir))); // Angle between viewer and surface normal
			edge = (edge < _OutlineWidth) ? (edge/_OutlineFalloff) : 1;
	
			float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = texColor.rgb * edge + _OutlineColor * (1.0-edge);
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