Shader "ARPG/2D/Sprite Selection Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        _OutlineColor("Outline Color", Color) = (1, 0.75, 0.1, 1)
        _OutlineThickness("Outline Thickness (Pixels)", Range(0, 8)) = 1

        // SpriteRenderer compatibility properties.
        [HideInInspector] _Color("Tint", Color) = (1, 1, 1, 1)
        [HideInInspector] _RendererColor("Renderer Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _Flip("Flip", Vector) = (1, 1, 1, 1)
        [PerRendererData] [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        TEXTURE2D(_NormalMap);
        SAMPLER(sampler_NormalMap);

        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        float4 _NormalMap_ST;
        float4 _Color;
        half4 _RendererColor;
        half4 _OutlineColor;
        float _OutlineThickness;

        half SampleAlpha(float2 uv)
        {
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
        }

        half4 SampleOutlinedSprite(float2 uv, half4 tint)
        {
            half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * tint;
            float2 offset = _MainTex_TexelSize.xy * max(_OutlineThickness, 0.0);

            half neighbourAlpha = 0.0h;
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2( offset.x, 0.0)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2(-offset.x, 0.0)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2(0.0,  offset.y)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2(0.0, -offset.y)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2( offset.x,  offset.y)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2(-offset.x,  offset.y)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2( offset.x, -offset.y)));
            neighbourAlpha = max(neighbourAlpha, SampleAlpha(uv + float2(-offset.x, -offset.y)));
            neighbourAlpha *= tint.a;

            half outlineAlpha = saturate(neighbourAlpha - sprite.a) * _OutlineColor.a;
            half finalAlpha = saturate(sprite.a + outlineAlpha);

            // Convert the two coverage contributions back to a straight-alpha color.
            half3 weightedColor = sprite.rgb * sprite.a + _OutlineColor.rgb * outlineAlpha;
            half3 finalColor = weightedColor / max(finalAlpha, 0.0001h);

            return half4(finalColor, finalAlpha);
        }
        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile_instancing
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                half2 lightingUV : TEXCOORD1;
                #if defined(DEBUG_DISPLAY)
                float3 positionWS : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #ifdef UNITY_INSTANCING_ENABLED
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteFlip);
                #endif

                output.positionCS = TransformObjectToHClip(input.positionOS);
                #if defined(DEBUG_DISPLAY)
                output.positionWS = TransformObjectToWorld(input.positionOS);
                #endif
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.lightingUV = half2(ComputeScreenPos(output.positionCS / output.positionCS.w).xy);
                output.color = input.color * _Color * _RendererColor;

                #ifdef UNITY_INSTANCING_ENABLED
                output.color *= unity_SpriteColor;
                #endif

                return output;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings input) : SV_Target
            {
                half4 main = SampleOutlinedSprite(input.uv, input.color);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);

                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);

                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering" }

            HLSLPROGRAM
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half3 tangentWS : TEXCOORD2;
                half3 bitangentWS : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings NormalsRenderingVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #ifdef UNITY_INSTANCING_ENABLED
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteFlip);
                #endif

                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _NormalMap);
                output.color = input.color * _Color * _RendererColor;
                output.normalWS = -GetViewForwardDir();
                output.tangentWS = TransformObjectToWorldDir(input.tangent.xyz);
                output.bitangentWS = cross(output.normalWS, output.tangentWS) * input.tangent.w;

                #ifdef UNITY_INSTANCING_ENABLED
                output.color *= unity_SpriteColor;
                #endif

                return output;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                half4 main = SampleOutlinedSprite(input.uv, input.color);
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));
                return NormalsRenderingShared(main, normalTS, input.tangentWS, input.bitangentWS, input.normalWS);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings UnlitVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #ifdef UNITY_INSTANCING_ENABLED
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteFlip);
                #endif

                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color * _RendererColor;

                #ifdef UNITY_INSTANCING_ENABLED
                output.color *= unity_SpriteColor;
                #endif

                return output;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                return SampleOutlinedSprite(input.uv, input.color);
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/2D/Sprite-Lit-Default"
}
