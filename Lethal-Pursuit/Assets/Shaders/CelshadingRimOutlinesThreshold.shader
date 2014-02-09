Shader "Badass VFX/Celshading (Rim Outlines, Threshold)" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
		_OutlineWidth ("Outline Width", Range(0, 1)) = 0.4
		_ColorMerge ("Color Merge", Range(0.1, 20000)) = 8
	}

	SubShader {

		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		// Upgrade NOTE: excluded shader from Xbox360 because it uses wrong array syntax (type[size] name)
		#pragma exclude_renderers xbox360
		#pragma surface surf Celshaded finalcolor:final
		
		sampler2D _Ramp;
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
			// Create outlines using black rim lighting
			half edge = saturate(dot (o.Normal, normalize(IN.viewDir)));
			edge = (edge < _OutlineWidth) ? (edge/4) : 1;
	
			float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = floor(texColor.rgb * _ColorMerge)/_ColorMerge * edge;
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
			color = _MainColor*floor(color * _NumColors)/_NumColors;
		}

		ENDCG
	}
	
	
	Fallback "Diffuse"
}