Shader "IndieEffects/RadialBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Blur("SampleSize", float) = 0.65
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Pass {
			Blend OneMinusSrcAlpha SrcAlpha
			ColorMask RGB
			SetTexture [_MainTex] {
				ConstantColor(1,1,1,[_Blur])
				Combine texture, constant
			}
		}
	}
}
