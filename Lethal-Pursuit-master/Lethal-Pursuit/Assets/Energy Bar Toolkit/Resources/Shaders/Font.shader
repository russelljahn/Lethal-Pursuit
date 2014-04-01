Shader "Energy Bar Toolkit/Unlit/Font" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_PrimaryColor ("Primary Color (White)", Color) = (1, 1, 1, 1)
		_SecondaryColor ("Secondary Color (Black)", Color) = (0, 0, 0, 1)
    }

    SubShader {
        Tags {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 100
 
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off
        Lighting Off
        ColorMaterial AmbientAndDiffuse
        
		CGPROGRAM

		#pragma surface surf NoLighting noforwardadd noambient

		sampler2D _MainTex;
		fixed4 _PrimaryColor;
		fixed4 _SecondaryColor;

		struct Input {
			fixed2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _PrimaryColor.rgb + ((1 - c.rgb) * _SecondaryColor.rgb);
			o.Alpha = c.a;
		}

		half4 LightingNoLighting(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }
		ENDCG
    }
}
