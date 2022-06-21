// see README here: 
// github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader

//URP基于屏幕空间的贴花shader
Shader "MyLittleCase/Screen Space Decal/Unlit"
{
    Properties
    {
        //主纹理，颜色
        [Header(Basic)]
        [MainTexture]_MainTex("Texture", 2D) = "white" {}
        [MainColor][HDR]_Color("_Color (default = 1,1,1,1)", Color) = (1,1,1,1)

        [Header(Blending)]
        // https://docs.unity3d.com/ScriptReference/Rendering.BlendMode.html
        //源混合模式，目标混合模式
        [Enum(UnityEngine.Rendering.BlendMode)]_DecalSrcBlend("_DecalSrcBlend (default = SrcAlpha)", Int) = 5 // 5 = SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]_DecalDstBlend("_DecalDstBlend (default = OneMinusSrcAlpha)", Int) = 10 // 10 = OneMinusSrcAlpha

        [Header(Alpha remap(extra alpha control))]
        _AlphaRemap("_AlphaRemap (default = 1,0,0,0) _____alpha will first mul x, then add y    (zw unused)", vector) = (1,0,0,0)

        [Header(Prevent Side Stretching(Compare projection direction with scene normal and Discard if needed))]
        [Toggle(_ProjectionAngleDiscardEnable)] _ProjectionAngleDiscardEnable("_ProjectionAngleDiscardEnable (default = off)", float) = 0
        _ProjectionAngleDiscardThreshold("_ProjectionAngleDiscardThreshold (default = 0)", range(-1,1)) = 0

        [Header(Mul alpha to rgb)]
        [Toggle]_MulAlphaToRGB("_MulAlphaToRGB (default = off)", Float) = 0

        [Header(Ignore texture wrap mode setting)]
        [Toggle(_FracUVEnable)] _FracUVEnable("_FracUVEnable (default = off)", Float) = 0

        //====================================== below = usually can ignore in normal use case =====================================================================
        [Header(Stencil Masking)]
        // https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        _StencilRef("_StencilRef", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("_StencilComp (default = Disable) _____Set to NotEqual if you want to mask by specific _StencilRef value, else set to Disable", Float) = 0 //0 = disable

        [Header(ZTest)]
        // https://docs.unity3d.com/ScriptReference/Rendering.CompareFunction.html
        // default need to be Disable, because we need to make sure decal render correctly even if camera goes into decal cube volume, although disable ZTest by default will prevent EarlyZ (bad for GPU performance)
        //默认禁用，因为我们需要保证贴花相机即使进入贴花立方体的体积内，贴花渲染让然正确，尽管默认禁用ZTest将会阻止EarlyZ（不利于GPU性能）
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("_ZTest (default = Disable) _____to improve GPU performance, Set to LessEqual if camera never goes into cube volume, else set to Disable", Float) = 0 //0 = disable

        [Header(Cull)]
        // https://docs.unity3d.com/ScriptReference/Rendering.CullMode.html
        // default need to be Front, because we need to make sure decal render correctly even if camera goes into decal cube
        //CullMode,默认为Front, 因为我们需要确保即使相机进入贴花立方体体积，贴花渲染也正确
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("_Cull (default = Front) _____to improve GPU performance, Set to Back if camera never goes into cube volume, else set to Front", Float) = 1 //1 = Front

        [Header(Unity Fog)]
        [Toggle(_UnityFogEnable)] _UnityFogEnable("_UnityFogEnable (default = on)", Float) = 1

        [Header(Support Orthographic camera)]
        [Toggle(_SupportOrthographicCamera)] _SupportOrthographicCamera("_SupportOrthographicCamera (default = off)", Float) = 0
    }

    SubShader
    {
        // To avoid render order problems, Queue must >= 2501, which enters the transparent queue, 
        // in transparent queue Unity will always draw from back to front
        // https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader/issues/6#issuecomment-615940985

        // https://docs.unity3d.com/Manual/SL-SubShaderTags.html
        // Queues up to 2500 (“Geometry+500”) are consided “opaque” and optimize the drawing order of the objects for best performance. 
        // Higher rendering queues are considered for “transparent objects” and sort objects by distance, 
        // starting rendering from the furthest ones and ending with the closest ones. 
        // Skyboxes are drawn in between all opaque and all transparent objects.
        // "Queue" = "Transparent-499" means "Queue" = "2501", which is almost equals "draw right before any transparent objects"

        // "DisableBatching" means disable "dynamic batching", not "srp batching"
        //-----------------------------------------------------------------------------------------------------
        //为了避免渲染顺序的问题，渲染队列必须>=2501，让他位于透明的渲染队列中
        //在透明渲染队列中，Unity总是从后向前进行渲染
        //2500以下是不透明的渲染队列，会进行渲染优化，比如被遮挡的地方就将其剔除，不进行渲染
        //2500以上是透明渲染队列，他会根据距离相机的距离进行排序
        //从最远开始，到最近结束
        //天空盒被渲染在所有不透明和透明物体之间
        //"Queue" = "Transparent-499"即“Queue = 2501”，这让他早于所有透明物体进行渲染
        Tags 
        { 
            "RenderType" = "Overlay" 
            "Queue" = "Transparent-499" 
            "DisableBatching" = "True" 
        }

        Pass
        {
            Stencil
            {
                Ref[_StencilRef]
                Comp[_StencilComp]
            }

            Cull[_Cull]
            ZTest[_ZTest]

            //为了支持透明混合，关闭深度写入
            ZWrite off
            //决定混合模式
            Blend[_DecalSrcBlend][_DecalDstBlend]

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            //让雾效生效
            #pragma multi_compile_fog

            // due to using ddx() & ddy()
            //使用ddx和ddy
            #pragma target 3.0

            #pragma shader_feature_local_fragment _ProjectionAngleDiscardEnable
            #pragma shader_feature_local _UnityFogEnable
            #pragma shader_feature_local_fragment _FracUVEnable
            #pragma shader_feature_local_fragment _SupportOrthographicCamera

            // Required by all Universal Render Pipeline shaders.
            // It will include Unity built-in shader variables (except the lighting variables)
            // (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
            // It will also include many utilitary functions. \

            //所有URP Pipeline的shader都位于Core.hlsl中
            //它包含了Unity内置的shader变量（除了光照相关的变量）
            //它同样也包含许多工具函数
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                //模型空间坐标
                float3 positionOS : POSITION;
            };

            struct v2f
            {
                //裁剪坐标
                float4 positionCS : SV_POSITION;
                //屏幕坐标
                float4 screenPos : TEXCOORD0;
                //xyz:模型空间下相机到顶点的射线，w:观察空间下z的副本
                float4 viewRayOS : TEXCOORD1; // xyz: viewRayO, w: extra copy of positionVS.z 
                //模型空间相机位置，a：雾效强度
                float4 cameraPosOSAndFogFactor : TEXCOORD2;
            };

            //主纹理
            sampler2D _MainTex;
            //相机深度图
            sampler2D _CameraDepthTexture;

            //开启SRP Batcher
            CBUFFER_START(UnityPerMaterial)               
                float4 _MainTex_ST;
                float _ProjectionAngleDiscardThreshold;
                half4 _Color;
                half2 _AlphaRemap;
                half _MulAlphaToRGB;
            CBUFFER_END

            //顶点着色器
            v2f vert(appdata input)
            {
                v2f o;

                // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space, ndc)
                // Unity compiler will strip all unused references (say you don't use view space).
                // Therefore there is more flexibility at no additional cost with this struct.

                //VertexPositionInputs类型包含一个顶点在多个空间坐标系中的位置(world，view,homogeneous clip space,ndc空间)
                //Unity编译器将会剥离所有未使用的引用（比如你没有使用view space）
                //因此这种结构更有灵活性，无需额外成本
                VertexPositionInputs vertexPositionInput = GetVertexPositionInputs(input.positionOS);

                //得到clip space（其次裁剪空间）下的顶点坐标j
                o.positionCS = vertexPositionInput.positionCS;

                // regular unity fog
                //雾效处理
#if _UnityFogEnable
                o.cameraPosOSAndFogFactor.a = ComputeFogFactor(o.positionCS.z);
#else
                o.cameraPosOSAndFogFactor.a = 0;
#endif

                // prepare depth texture's screen space UV
                //准备深度纹理的屏幕空间UV
                //ComputeScreenPos函数：传入齐次坐标，返回屏幕坐标
                o.screenPos = ComputeScreenPos(o.positionCS);

                // get "camera to vertex" ray in View space
                //观察空间中相机到顶点的射线向量
                float3 viewRay = vertexPositionInput.positionVS;

                // [important note]
                //=========================================================
                // "viewRay z division" must do in the fragment shader, not vertex shader! (due to rasteriazation varying interpolation's perspective correction)
                // We skip the "viewRay z division" in vertex shader for now, and store the division value into varying o.viewRayOS.w first, 
                // we will do the division later when we enter fragment shader
                // viewRay /= viewRay.z; //skip the "viewRay z division" in vertex shader for now

                //关键步骤
                //viewRay除以z分量必须在片元着色器中执行，不能跑在顶点着色器，这是由于光栅化插值的透视校正
                //先将viewRay.z保存到o.viewRayOS.w中，以便在片元着色器中处理
                o.viewRayOS.w = viewRay.z;//store the division value to varying o.viewRayOS.w
                //=========================================================

                // unity's camera space is right hand coord(negativeZ pointing into screen), we want positive z ray in fragment shader, so negate it
                //unity相机空间使用右手坐标系，（Z轴负向指向屏幕），希望它在片元着色器中是正确的，所以取反
                viewRay *= -1;

                // it is ok to write very expensive code in decal's vertex shader, 
                // it is just a unity cube(4*6 vertices) per decal only, won't affect GPU performance at all.
                //得到观察空间到模型空间的变换矩阵
                float4x4 ViewToObjectMatrix = mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V);

                // transform everything to object space(decal space) in vertex shader first, so we can skip all matrix mul() in fragment shader
                //将相机射线由观察空间变换到模型空间
                o.viewRayOS.xyz = mul((float3x3)ViewToObjectMatrix, viewRay);
                //得到模型空间下的相机坐标位置
                o.cameraPosOSAndFogFactor.xyz = mul(ViewToObjectMatrix, float4(0,0,0,1)).xyz; // hard code 0 or 1 can enable many compiler optimization

                return o;
            }

            // copied from URP12.1.2's ShaderVariablesFunctions.hlsl
            float LinearDepthToEyeDepth(float rawDepth)
            {
                #if UNITY_REVERSED_Z
                    return _ProjectionParams.z - (_ProjectionParams.z - _ProjectionParams.y) * rawDepth;
                #else
                    return _ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y) * rawDepth;
                #endif
            }

            half4 frag(v2f i) : SV_Target
            {
                // [important note]
                //========================================================================
                // now do "viewRay z division" that we skipped in vertex shader earlier.
                i.viewRayOS.xyz /= i.viewRayOS.w;
                //========================================================================

                float2 screenSpaceUV = i.screenPos.xy / i.screenPos.w;
                float sceneRawDepth = tex2D(_CameraDepthTexture, screenSpaceUV).r;

                float3 decalSpaceScenePos;

#if _SupportOrthographicCamera
                // we have to support both orthographic and perspective camera projection
                // static uniform branch depends on unity_OrthoParams.w
                // (should we use UNITY_BRANCH here?) decided NO because https://forum.unity.com/threads/correct-use-of-unity_branch.476804/
                if(unity_OrthoParams.w)
                {
                    float sceneDepthVS = LinearDepthToEyeDepth(sceneRawDepth);

                    //***Used a few lines from Asset: Lux URP Essentials by forst***
                    // Edit: The copied Lux URP stopped working at some point, and no one even knew why it worked in the first place 
                    //----------------------------------------------------------------------------
                    float2 viewRayEndPosVS_xy = float2(unity_OrthoParams.xy * (i.screenPos.xy - 0.5) * 2 /* to clip space */);  // Ortho near/far plane xy pos 
                    float4 vposOrtho = float4(viewRayEndPosVS_xy, -sceneDepthVS, 1);                                            // Constructing a view space pos
                    float3 wposOrtho = mul(UNITY_MATRIX_I_V, vposOrtho).xyz;                                                 // Trans. view space to world space
                    //----------------------------------------------------------------------------

                    // transform world to object space(decal space)
                    decalSpaceScenePos = mul(GetWorldToObjectMatrix(), float4(wposOrtho, 1)).xyz;
                }
                else
                {
#endif
                    // if perspective camera, LinearEyeDepth will handle everything for user
                    // remember we can't use LinearEyeDepth for orthographic camera!
                    float sceneDepthVS = LinearEyeDepth(sceneRawDepth,_ZBufferParams);

                    // scene depth in any space = rayStartPos + rayDir * rayLength
                    // here all data in ObjectSpace(OS) or DecalSpace
                    // be careful, viewRayOS is not a unit vector, so don't normalize it, it is a direction vector which view space z's length is 1
                    decalSpaceScenePos = i.cameraPosOSAndFogFactor.xyz + i.viewRayOS.xyz * sceneDepthVS;
                    
#if _SupportOrthographicCamera
                }
#endif

                // convert unity cube's [-0.5,0.5] vertex pos range to [0,1] uv. Only works if you use a unity cube in mesh filter!
                float2 decalSpaceUV = decalSpaceScenePos.xy + 0.5;

                // discard logic
                //===================================================
                // discard "out of cube volume" pixels
                float shouldClip = 0;
#if _ProjectionAngleDiscardEnable
                // also discard "scene normal not facing decal projector direction" pixels
                float3 decalSpaceHardNormal = normalize(cross(ddx(decalSpaceScenePos), ddy(decalSpaceScenePos)));//reconstruct scene hard normal using scene pos ddx&ddy

                // compare scene hard normal with decal projector's dir, decalSpaceHardNormal.z equals dot(decalForwardDir,sceneHardNormalDir)
                shouldClip = decalSpaceHardNormal.z > _ProjectionAngleDiscardThreshold ? 0 : 1;
#endif
                // call discard
                // if ZWrite is Off, clip() is fast enough on mobile, because it won't write the DepthBuffer, so no GPU pipeline stall(confirmed by ARM staff).
                clip(0.5 - abs(decalSpaceScenePos) - shouldClip);
                //===================================================

                // sample the decal texture
                float2 uv = decalSpaceUV.xy * _MainTex_ST.xy + _MainTex_ST.zw;//Texture tiling & offset
#if _FracUVEnable
                uv = frac(uv);// add frac to ignore texture wrap setting
#endif
                half4 col = tex2D(_MainTex, uv);
                col *= _Color;// tint color
                col.a = saturate(col.a * _AlphaRemap.x + _AlphaRemap.y);// alpha remap MAD
                col.rgb *= lerp(1, col.a, _MulAlphaToRGB);// extra multiply alpha to RGB

#if _UnityFogEnable
                // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
                // with a custom one.
                col.rgb = MixFog(col.rgb, i.cameraPosOSAndFogFactor.a);
#endif
                return col;
            }
            ENDHLSL
        }
    }
}