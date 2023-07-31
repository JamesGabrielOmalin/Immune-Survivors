Shader "Custom/BillboardSprite_Unlit"
{
    Properties
    { 
        _MainTex("Texture", 2D) = "white" {}
        _ColorMask("Color Mask", 2D) = "white" {}
        [HDR]_Emissive("Emissive", Color) = (0,0,0,1)

        [Header(Alpha)]
        [Toggle(_UseAlphaClipping)]_UseAlphaClipping("UseAlphaClipping", Float) = 0
        _Cutoff("Cutoff (Alpha Cutoff)", Range(0.0, 1.0)) = 0.5
        [Toggle(_NoPixelFilter)]_NoPixelFilter("Disable Pixel Filter", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline" 
            "RenderType" = "Transparent" 
            "UniversalMaterialType" = "Unlit" 
            "Queue" = "Transparent" 
            "PreviewType" = "Plane"
            "IgnoreProjector" = "True"
            "DisableBatching" = "True"
        }
        HLSLINCLUDE

        // all Passes will need this keyword
        #pragma shader_feature_local_fragment _UseAlphaClipping
        #pragma shader_feature_local_fragment _NoPixelFilter
        ENDHLSL

        Pass
        {
            Name "ForwardLit"

            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaToMask Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag_unlit

            #define _MAIN_LIGHT_SHADOWS

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile_fog

            #include "BillboardSprite_Shared.hlsl"
            ENDHLSL
        }

        Pass
        {
           Name "ShadowCaster"
           Tags{"LightMode" = "ShadowCaster"}

           ZWrite On
           ZTest LEqual          
           ColorMask 0
           Cull Off

            HLSLPROGRAM
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex vert
            #pragma fragment BaseColorAlphaClipTest

            #define ApplyShadowBiasFix

            #include "BillboardSprite_Shared.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment BaseColorAlphaClipTest

            #include "BillboardSprite_Shared.hlsl"
            ENDHLSL
        }
    }
}