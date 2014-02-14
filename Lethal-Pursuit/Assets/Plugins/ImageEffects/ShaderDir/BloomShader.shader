Shader "Custom/BloomShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurTex ("BlurringTex (RGB)", 2D) = "white" {}
		_Amount ("Bloom Amount", Range(0,2)) = 0.5
		_Threshold ("Threshold", float) = 0.8
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _BlurTex;
	float4 _MainTex_TexelSize;
	float _Amount;
	float _Threshold;
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	struct v2f_off {
		float4 pos : POSITION;
		float2 uv[8] : TEXCOORD0;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

		o.uv = v.texcoord.xy;
		
		return o;
	}
	
	v2f_off vertOff (appdata_img v)
	{
		v2f_off o;
		
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

		float2 uv = v.texcoord.xy;
				
		float2 up = float2(0.0, _MainTex_TexelSize.y) * 3;
		float2 right = float2(_MainTex_TexelSize.x, 0.0) * 3;	
			
		o.uv[0].xy = uv + up;
		o.uv[1].xy = uv - up;
		o.uv[2].xy = uv + right;
		o.uv[3].xy = uv - right;
		o.uv[4].xy = uv - right + up;
		o.uv[5].xy = uv - right -up;
		o.uv[6].xy = uv + right + up;
		o.uv[7].xy = uv + right -up;
		
		return o;
	}
	
	half4 frag (v2f i) : COLOR
	{
		float4 col = tex2D(_MainTex, i.uv);
		
		if (Luminance( tex2D(_MainTex, i.uv).rgb ) >= _Threshold) {
			col.rgb = tex2D(_MainTex, i.uv).rgb;
		}
		
		return col;
	}
	
	half4 fragOff (v2f_off i) : COLOR
	{
		
		float4 col = tex2D(_MainTex, (i.uv[0] + i.uv[1]) * 0.5);
		float4 newCol;
		newCol.rgb = col.rgb;
		int count = 1;
		for (int pix = 0; pix < 8; pix ++) {
			if (i.uv[pix].x <= 1.0 && i.uv[pix].y <= 1.0 && i.uv[pix].x >= 0.0 && i.uv[pix].y >= 0.0) {
				newCol += tex2D(_BlurTex, i.uv[pix]) * _Amount;
				count ++;
			}
		}
		newCol = ((newCol + col) / (count + 1)) * _Amount;
		
		return newCol;
	}
	
	ENDCG
	
	SubShader {
		tags {"Queue" = "Overlay" "RenderType" = "Overlay"}
		ZTest Always Cull Off ZWrite Off
		pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
		pass {
		blend One One
			CGPROGRAM
			#pragma vertex vertOff
			#pragma fragment fragOff
			#pragma target 3.0
			ENDCG
		}
	}
}
