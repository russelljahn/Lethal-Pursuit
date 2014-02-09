Shader "Badass VFX/Celshading (Backface Outlines, Threshold)" {
	Properties {
		_MainColor ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_NumColors ("Number Of Colors To Use", Range(0.1, 30)) = 4
		_OutlineWidth ("Outline Width", Range(0, 1)) = 0.4
		_ColorMerge ("Color Merge", Range(0.1, 20000)) = 8
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
	
		Pass {	
		
//			Tags { "LightMode" = "ForwardBase" }
			Cull Back
			Lighting On
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform float4 _LightColor0;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _OutlineWidth;
			sampler2D _Ramp;
			float4 _MainColor;
			float _ColorMerge;
			float _NumColors;
			

			struct application2vertex {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			
			struct vertex2fragment {
				float4 position : POSITION;
				float2 uvs;
				float3 lightDirection;
				float3 normal;
			};


			vertex2fragment vert (application2vertex v) {
				vertex2fragment output;
				TANGENT_SPACE_ROTATION;
				
				output.lightDirection = mul(rotation, ObjSpaceLightDir(v.position));
				output.position = mul(UNITY_MATRIX_MVP, v.position); // Model-space -> Projection-space
				output.uvs = TRANSFORM_TEX(v.texcoord, _MainTex);
				
				output.normal = mul(UNITY_MATRIX_MVP, float4(v.normal, 0));
			
				
				return output;
			}
			
			
			float4 frag(vertex2fragment input) : COLOR {
				float4 color = tex2D(_MainTex, input.uvs);
				
				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				float lengthSquared = dot(input.lightDirection, input.lightDirection);
				float attenuation = 1.0/(1.0+lengthSquared*unity_LightAtten[0].z);
				
				float diffuseAmount = saturate(dot(input.normal, normalize(input.lightDirection)));
				lightColor += _LightColor0.rgb * diffuseAmount * attenuation;
				color.rgb = 2*color.rgb*lightColor;
				
				return color;
			}

//			void surf (Input IN, inout SurfaceOutput o) {
//				// Create outlines using black rim lighting
//				half edge = saturate(dot (o.Normal, normalize(IN.viewDir)));
//				edge = (edge < _OutlineWidth) ? (edge/4) : 1;
//		
//				float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
//				o.Albedo = floor(texColor.rgb * _ColorMerge)/_ColorMerge * edge;
//				o.Alpha = texColor.a;
//			 }
//			 
//			 
//			half4 LightingCelshaded (SurfaceOutput o, half3 lightDir, half attenuation) {
//				half4 lightColor;
//				half diffuseAmount = dot(o.Normal, lightDir);
//				diffuseAmount = floor(diffuseAmount * _NumColors)/_NumColors;
//				lightColor.rgb = o.Albedo * _LightColor0.rgb * diffuseAmount * attenuation * 2;
//				lightColor.a = o.Alpha;
//				return lightColor;
//			}
//			
//			
//			
//			void final (Input IN, SurfaceOutput o, inout fixed4 color) {
//				color = _MainColor*floor(color * _NumColors)/_NumColors;
//			}

			ENDCG
		}
		
//		Pass {
//			Tags { "LightMode" = "ForwardAdd" }
//			Cull Back
//			Lighting On
//			Blend One One
//			
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			
//			#include "UnityCG.cginc"
//			
//			uniform float4 _LightColor0;
//			
//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			float _OutlineWidth;
//			sampler2D _Ramp;
//			float4 _MainColor;
//			float _ColorMerge;
//			float _NumColors;
//			
//
//			struct application2vertex {
//				float4 position : POSITION;
//				float3 normal : NORMAL;
//				float4 texcoord : TEXCOORD0;
//				float4 tangent : TANGENT;
//			};
//			
//			
//			struct vertex2fragment {
//				float4 position : POSITION;
//				float2 uvs;
//				float3 lightDirection;
//				float3 normal;
//			};
//
//
//			vertex2fragment vert (application2vertex v) {
//				vertex2fragment output;
//				TANGENT_SPACE_ROTATION;
//				
//				output.lightDirection = mul(rotation, ObjSpaceLightDir(v.position));
//				output.position = mul(UNITY_MATRIX_MVP, v.position); // Model-space -> Projection-space
//				output.uvs = TRANSFORM_TEX(v.texcoord, _MainTex);
//				
//				output.normal = mul(UNITY_MATRIX_MVP, float4(v.normal, 0));
//				
//				return output;
//			}
//			
//			
//			float4 frag(vertex2fragment input) : COLOR {
//				float4 color = tex2D(_MainTex, input.uvs);
//				
//				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
//				
//				float lengthSquared = dot(input.lightDirection, input.lightDirection);
//				float attenuation = 1.0/(1.0+lengthSquared*unity_LightAtten[0].z);
//				
//				float diffuseAmount = saturate(dot(input.normal, normalize(input.lightDirection)));
//				lightColor += _LightColor0.rgb * diffuseAmount * attenuation;
//				color.rgb = 2*color.rgb*lightColor;
//				
//				return color;
//			}
//			
//			ENDCG
//		}
	}
	
	
	Fallback "Diffuse"
}