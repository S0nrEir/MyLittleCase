//世界坐标重建测试shader
Shader "MyLittleCase/WorldPositionShader"
{
    //Properties
    //{
    //}
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipleline" = "UniversalPipeline"
        }
        LOD 200

        Pass
        {
            HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

            ENDHLSL

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                return half4(i.worldPos, 1.0);

            }

            ENDHLSL
        }
    }
}
