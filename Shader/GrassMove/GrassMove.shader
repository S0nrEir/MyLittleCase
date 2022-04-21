//草地摆动和压弯效果
Shader "MyLittleCase/GrassMove"
{
    Properties
    {
    	//主贴图
        _MainTex ("Main Tex", 2D) = "white" {}
        //噪点图
        _Noise("Noise Tex",2D) = "white"{}
        //颜色
        _Color("Main Tint",Color) = (1,1,1,1)
        //摆动系数
        _WaveFactor("BlindFactor",Range(1,30)) = 10
        //前面几个分量表示在各个轴向上自身摆动的速度, w表示摆动的强度
        _WaveControl("Wave Control(X_Speed,YS_peed,Z_Speed,W_World_Size)",vector) = (1,0,1,1)
        //风的摆动方向,w代表强度
        _WindControl("Wind Control",vector) = (1,1,1,1)
        //玩家位置，世界坐标
        //_PlayerPos("Player World Position",vector) = (0,0,0)
        //影响范围
        _Radius("Radius",float) = 1.0
        //强度
        _Strength("Strength",float) = 1.0

    }
    SubShader
    {
        Tags 
        { 
        	"RenderType"="Opaque"
        	//使用URP进行处理
        	"RenderPipeline" = "UniversalPipeline"
    	}

        Pass
        {
        	Tags
        	{
        		"LightMode" = "UniversalForward"
        	}

    		LOD 200

            HLSLINCLUDE

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

			//主纹理
            float4 _MainTex_ST;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            //噪点图
            float4 _Noise_ST;
            sampler2D _Noise;

            //
            float4 _Color;
            float _WaveFactor;
            float4 _WaveControl;
            float4 _WindControl;
            float _Radius;
            float _Strength;
            float4 _PlayerPos;

            ENDHLSL

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct a2v
            {
            	float4 vertex : POSITION;
            	float4 texcoord : TEXCOORD0;
            	float2 uv : TEXCOORD1;
            };

            struct v2f
            {
            	float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                //用来测试传递噪点图采样结果
            	//float2 tempCol : TEXCOORD2;
            };

            //vertex shader
            v2f vert (a2v v)
            {
            	v2f o;
            	//模型顶点变换到世界坐标做个偏移再给到fs
            	float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

            	float2 sample_pos = worldPos.xz / _WaveControl.w;
            	//噪点图的采样点
            	sample_pos += _Time.x / 5 * _WaveControl.xz;

            	//对于lod采样，要用采样器sampler_##name
            	//在hlsl中，使用lod贴图采样需将采样贴图声明为内置渲染管线格式
            	//采样噪点图作为决定风摆
            	half wave_sample = tex2Dlod(_Noise,float4(sample_pos , 0, 0)).r;
            	//o.tempCol = wave_sample;
 
            	worldPos.x += sin(_WindControl.x * wave_sample * _WaveFactor) * v.uv.y * _WaveControl.x * _WindControl.x;
            	worldPos.z += sin(_WindControl.z * wave_sample * _WaveFactor) * v.uv.y * _WaveControl.z * _WindControl.z;

            	//这里直接给世界坐标然后得到距离
            	float dist = distance(_PlayerPos,worldPos);
            	//计算压弯程度，0为完全不压弯（即在影响范围外）
            	float push_down = saturate(1 - dist + _Radius) * v.uv.x * _Strength;
            	//计算外扩方向
            	float3 direction = normalize(worldPos.xyz - _PlayerPos.xyz);
            	//减少y轴上的影响程度
            	direction.y *= 0.5;
            	worldPos.xyz += direction * push_down;

            	o.worldPos = worldPos.xyz;
            	o.pos = mul(UNITY_MATRIX_VP, worldPos);
            	o.uv = TRANSFORM_TEX(v.texcoord , _MainTex);
            	return o;
            }

            //fragment shader
            float4 frag (v2f i) : SV_Target
            {
            	half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            	return float4(texColor.rgb * _Color.rgb ,1);
            }

            ENDHLSL
        }
    }
}
