
Shader "BadassVFX/Negative" {
	Properties {
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "Queue"="Transparent" }

		Pass {
			ZWrite On
			ColorMask 0
		}
		Pass {
			Blend OneMinusDstColor OneMinusSrcAlpha
			BlendOp Add
			SetTexture [_Color] {
				constantColor [_Color]
				combine constant
			}
		}
		
	}
}