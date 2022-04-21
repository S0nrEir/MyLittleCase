//地形扫描器
Shader "MyLittleCase/TerrainScan"
{
	Properties
	{
		//后处理渲染图片采样纹理
		[HideInInspector]_MainTex("_MainTex",2D) = "white"{}
		//Distance和Width用来控制depth的显示
		//距离
		_Distance("Distance",Range(0,0.01)) = 0.005
		//宽度
		_Width("Width",Range(0,0.1)) = 0.05

		//扫描线颜色
		_ScanLineColor("ScanLineColor",Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RednerPipeline" = "UniversalPipeline"
		}

		Pass
		{
			Tags { "LightMode" = "UniversalForward" }
			LOD 200

			HLSLINCLUDE

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

			//深度图的获取也包含在这里面
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			//深度图
			//TEXTURE2D(_CameraDepthTexture)
			//TEXTURE2D_X_FLOAT(_CameraDepthTexture);
			//SAMPLER(sampler_CameraDepthTexture);

			float4 _MainText_ST;
			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			float _Distance;
			float _Width;
			half4 _ScanLineColor;

			ENDHLSL

			HLSLPROGRAM

			struct a2v 
			{
				float4 vertex : POSITION;//模型顶点位置
				float2 texcoord: TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screen_pos : TEXCOORD1;
			};

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(a2v v)
			{
				v2f o;

				o.pos = TransformObjectToHClip(v.vertex);
				o.uv = v.texcoord;
				o.screen_pos = ComputeScreenPos(o.pos);

				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				//深度图采样
				// half4 depth = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,i.uv);
				// half4 pixel_color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				// return depth + pixel_color;

				// half depth = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,i.uv).r;
				// //在HLSL中，Linear01Depth的参数由内置管线中的一个变成了两个，第二个参数传入_ZBufferParams，不需要提前声明
				// //Linear01Depth函数将深度转换成范围在[0,1]的线性变化深度值
				// depth = Linear01Depth(depth, _ZBufferParams);
				// return depth * 100;

				half depth = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,i.uv).r;
				depth = Linear01Depth(depth,_ZBufferParams);
				half4 pixel_color = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
				//在屏幕中根据深度画出扫描线
				//如果当前像素点的深度在width和distance范围内，则画线
				if(depth > _Distance && depth < _Distance + _Width)
				{
					depth = smoothstep(_Distance,_Distance + _Width, depth);
					return depth * _ScanLineColor + pixel_color ;
				}

				//return depth + SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv);
				 return pixel_color;

			}

			ENDHLSL
		}
	}
}