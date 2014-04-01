Shader "BadassVFX/Graphic" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
//		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
		_Brightness ("Brightness Factor", Range(1, 10)) = 1
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Lighting On
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong finalcolor:final

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float4 _MainColor;
		float _NumColors;
		float _Brightness;
		
		
		
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 texColor = tex2D (_MainTex, IN.uv_MainTex);
		
			half4 noiseAmount = tex2D (_NoiseTex, IN.uv_MainTex);
			float noise = noiseAmount.x *cos(noiseAmount.y);
			
			float speed1 = 0.5;
			
			float4 color = 
				float4(
					speed1*noise*IN.viewDir.x*IN.worldNormal.x, 
					speed1*noise*IN.viewDir.y*IN.worldNormal.y, 
					speed1*noise*IN.viewDir.z*IN.worldNormal.z,
					1
				);
		
			
			float4 finalColor = saturate(float4(0.25, 0.25, 0.25, 1.0) + 0.5*color + texColor);
			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;
		}
		
		void final (Input IN, SurfaceOutput o, inout fixed4 color) {
			color = _MainColor*floor(color * _NumColors)/_NumColors * _Brightness;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
