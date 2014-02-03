Shader "IndieEffects/GodRays" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Intensity("SampleSize", float) = 0.65
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Pass {
			Blend OneMinusSrcAlpha SrcAlpha
			ColorMask RGB
			SetTexture [_MainTex] {
				ConstantColor(1,1,1,0.7)
				Combine texture, constant
			}
		}
		Pass {
			Blend DstColor One
			ColorMask RGB
			SetTexture [_MainTex] {
				ConstantColor(1,1,1,[_Intensity])
				Combine texture, constant
			}
		}
	}
}
