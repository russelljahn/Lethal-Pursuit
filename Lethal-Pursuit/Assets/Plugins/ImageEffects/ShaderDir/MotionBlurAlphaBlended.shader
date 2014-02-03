Shader "IndieEffects/Alpha-Blended Motion Blur" {
	Properties {
		_MainTex ("Texture(RGB)", 2D) = "black" {}
		_Sample2 ("Texture2(RGB)", 2D) = "black" {}
		_FrameAccumulation ("Accumulation", float) = 0.65
	}
	
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			SetTexture [_MainTex] {
				ConstantColor(1,1,1,[_FrameAccumulation])
				Combine texture, constant
			}
		}
	}
	Fallback off
}