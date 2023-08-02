#pragma once

// Required by all Universal Render Pipeline shaders.
// It will include Unity built-in shader variables (except the lighting variables)
// (https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
// It will also include many utilitary functions. 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Include this if you are doing a lit shader. This includes lighting shader variables,
// lighting and shadow functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif


struct Attributes
{
    float4 positionOS : POSITION;
    half3 normalOS : NORMAL;
    half4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
    float4 color : COLOR0;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionWSAndFogFactor : TEXCOORD1; // xyz: positionWS, w: vertex fog factor
    half3 normalWS : TEXCOORD2;
    half4 tangentWS : TEXCOORD3;
    float4 positionOS : TEXCOORD4;
    float4 positionCS : SV_POSITION;
    float4 color : COLOR0;
};

sampler2D _MainTex;
sampler2D _ColorMask;
CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
float4 _ColorMask_ST;
//float4 _BumpMap_ST;
half _Cutoff;
float _Fade;

CBUFFER_END

float3 _LightDirection;
float4 _MainTex_TexelSize;

struct SpriteSurfaceData
{
    half3 albedo;
    half alpha;
    half3 emission;
    half occlusion;
};
struct SpriteLightingData
{
    half3 normalWS;
    float3 positionWS;
    half3 viewDirectionWS;
    float4 shadowCoord;
    float3 positionOS;
};

float rayPlaneIntersection(float3 rayDir, float3 rayPos, float3 planeNormal, float3 planePos)
{
    float denom = dot(planeNormal, rayDir);
    denom = max(denom, 0.000001); // avoid divide by zero
    float3 diff = planePos - rayPos;
    return dot(diff, planeNormal) / denom;
}

Varyings vert(Attributes input)
{
    Varyings output;
    
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    
    float3 positionWS = vertexInput.positionWS;
    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    output.uv = TRANSFORM_TEX(input.uv.xy, _MainTex);
    output.color = input.color;
    
    float3 camPos = _WorldSpaceCameraPos;
    // world space mesh pivot
    float3 worldPivot = unity_ObjectToWorld._m03_m13_m23;
    
    float3 scale = float3(
                    length(unity_ObjectToWorld._m00_m01_m02),
                    length(unity_ObjectToWorld._m10_m11_m12),
                    1.0
                    );
    float a = 0;
//#ifdef ApplyShadowBiasFix
//    a = 1;
//#else
//    a = 0;
//#endif
    float3 f = normalize(lerp(
                    -UNITY_MATRIX_V[2].xyz, // view forward dir
                    normalize(worldPivot - camPos), // camera to pivot dir
                    a));
    float3 u = float3(0.0, 1.0, 0.0);
    float3 r = normalize(cross(u, f));
    u = -normalize(cross(r, f));
    float3x3 billboardRotation = float3x3(r, u, f);
    float3 worldPos = mul(input.positionOS.xyz * scale, billboardRotation) + worldPivot;
    
    // billboard mesh towards camera
    //float3 vpos = mul((float3x3) unity_ObjectToWorld, input.positionOS.xyz);

    //float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, -unity_ObjectToWorld._m23, 1);
    //float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);

    //output.positionCS = mul(UNITY_MATRIX_P, viewPos);
    output.positionCS = TransformWorldToHClip(worldPos);
    //output.normalWS = mul(UNITY_MATRIX_V, half3(0, 0, 1));
    billboardRotation[1] = -billboardRotation[1];
    output.normalWS = half3(0, 0, -1);
    
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(mul(billboardRotation, vertexNormalInput.tangentWS.xyz), sign);
    output.tangentWS = tangentWS;
    
    output.positionWSAndFogFactor = float4(positionWS, fogFactor);

    // calculate distance to vertical billboard plane seen at this vertex's screen position
    //float3 planeNormal = normalize(float3(UNITY_MATRIX_V._m20, 0.0, UNITY_MATRIX_V._m22));
    //float3 planePoint = unity_ObjectToWorld._m03_m13_m23;
    //float3 rayStart = _WorldSpaceCameraPos.xyz;
    //float3 rayDir = -normalize(mul(UNITY_MATRIX_I_V, float4(camPos.xyz, 1.0)).xyz - rayStart); // convert view to world, minus camera pos
    //float dist = rayPlaneIntersection(rayDir, rayStart, planeNormal, planePoint);

    //// calculate the clip space z for vertical plane
    //float4 planeOutPos = mul(UNITY_MATRIX_VP, float4(rayStart + rayDir * dist, 1.0));
    //float newPosZ = planeOutPos.z / planeOutPos.w * output.positionCS.w;

    // use the closest clip space z
//#if defined(UNITY_REVERSED_Z)
//    output.positionCS.z = max(output.positionCS.z, newPosZ);
//#else
//    output.positionCS.z = min(output.positionCS.z, newPosZ);
//#endif
    
#ifdef ApplyShadowBiasFix
    float4 positionCS = TransformWorldToHClip(worldPos);
    #if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif
    output.positionCS = positionCS;
#endif
    
    output.positionOS = input.positionOS;

    return output;
}

half4 GetFinalBaseColor(Varyings input)
{
#if _NoPixelFilter
    return tex2D(_MainTex, input.uv) * lerp(1, input.color, tex2D(_ColorMask, input.uv));
#endif

    float2 boxSize = clamp(fwidth(input.uv) * _MainTex_TexelSize.zw, 1e-5, 1);
    
    float2 tx = input.uv * _MainTex_TexelSize.zw - 0.5 * boxSize;
    
    float2 txOffset = smoothstep(1 - boxSize, 1, frac(tx));
    
    float2 uv = (floor(tx) + 0.5 + txOffset) * _MainTex_TexelSize.xy;
    
    float4 final = tex2Dgrad(_MainTex, uv, ddx(input.uv), ddy(input.uv)) * lerp(1, input.color, tex2Dgrad(_ColorMask, uv, ddx(input.uv), ddy(input.uv)).r);
    
    return final;
}

void DoClipTestToTargetAlphaValue(half alpha)
{
#if _UseAlphaClipping
    clip(alpha - _Cutoff);
#endif
}

SpriteSurfaceData InitializeSurfaceData(Varyings input)
{
    SpriteSurfaceData output = (SpriteSurfaceData)0;

    // albedo & alpha
    float4 baseColorFinal = GetFinalBaseColor(input);
    output.albedo = baseColorFinal.rgb;
    output.alpha = baseColorFinal.a;
    //DoClipTestToTargetAlphaValue(output.alpha); // early exit if possible

    // emission
    //output.emission = GetFinalEmissionColor(input);

    // occlusion
    //output.occlusion = GetFinalOcculsion(input);

    return output;
}

SpriteLightingData InitializeLightingData(Varyings input)
{
    float2 boxSize = clamp(fwidth(input.uv) * _MainTex_TexelSize.zw, 1e-5, 1);
    
    float2 tx = input.uv * _MainTex_TexelSize.zw - 0.5 * boxSize;
    
    float2 txOffset = smoothstep(1 - boxSize, 1, frac(tx));
    
    float2 uv = (floor(tx) + 0.5 + txOffset) * _MainTex_TexelSize.xy;
    
    SpriteLightingData lightingData;
    lightingData.positionWS = input.positionWSAndFogFactor.xyz;
    lightingData.positionOS = input.positionOS;
    lightingData.viewDirectionWS = SafeNormalize(GetCameraPositionWS() - lightingData.positionWS);
    
    half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), 1);
    
#if defined(_NORMALMAP) || defined(_DETAIL)
    float sgn = input.tangentWS.w;      // should be either +1 or -1
    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

#if defined(_NORMALMAP)
    //lightingData.tangentToWorld = tangentToWorld;
#endif 
    lightingData.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
#else
    lightingData.normalWS = input.normalWS;
#endif
    
    return lightingData;
}

#include "BillboardSprite_Lighting.hlsl"

half3 ShadeAllLights(SpriteSurfaceData surfaceData, SpriteLightingData lightingData)
{
    // Indirect lighting
    half3 indirectResult = ShadeGI(surfaceData, lightingData);

    //////////////////////////////////////////////////////////////////////////////////
    // Light struct is provided by URP to abstract light shader variables.
    // It contains light's
    // - direction
    // - color
    // - distanceAttenuation 
    // - shadowAttenuation
    //
    // URP take different shading approaches depending on light and platform.
    // You should never reference light shader variables in your shader, instead use the 
    // -GetMainLight()
    // -GetLight()
    // funcitons to fill this Light struct.
    //////////////////////////////////////////////////////////////////////////////////

    //==============================================================================================
    // Main light is the brightest directional light.
    // It is shaded outside the light loop and it has a specific set of variables and shading path
    // so we can be as fast as possible in the case when there's only a single directional light
    // You can pass optionally a shadowCoord. If so, shadowAttenuation will be computed.
    Light mainLight = GetMainLight();

    //float3 shadowTestPosWS = lightingData.positionWS + mainLight.direction;// * (_ReceiveShadowMappingPosOffset + _IsFace);
    
    //float3 vpos = mul((float3x3) unity_ObjectToWorld, lightingData.positionOS.xyz);
    //float4 shadowTestPosWS = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
    //shadowTestPosWS.xyz += mul((float3x3) unity_CameraToWorld, vpos);
    
    float4 shadowTestPosWS = float4(lightingData.positionWS + mainLight.direction, 1);
    
#ifdef _MAIN_LIGHT_SHADOWS
    // compute the shadow coords in the fragment shader now due to this change
    // https://forum.unity.com/threads/shadow-cascades-weird-since-7-2-0.828453/#post-5516425

    // _ReceiveShadowMappingPosOffset will control the offset the shadow comparsion position, 
    // doing this is usually for hide ugly self shadow for shadow sensitive area like face
    float4 shadowCoord = TransformWorldToShadowCoord(shadowTestPosWS);
    half4 shadowMask = unity_ProbesOcclusion;
    mainLight.shadowAttenuation = MainLightShadow(shadowCoord, shadowTestPosWS, shadowMask, _MainLightOcclusionProbes);
#endif 

    // Main light
    half3 mainLightResult = ShadeSingleLight(surfaceData, lightingData, mainLight, false);
    

    //==============================================================================================
    // All additional lights

    half3 additionalLightSumResult = 0;

#ifdef _ADDITIONAL_LIGHTS
    // Returns the amount of lights affecting the object being renderer.
    // These lights are culled per-object in the forward renderer of URP.
    int additionalLightsCount = GetAdditionalLightsCount();
    for (int i = 0; i < additionalLightsCount; ++i)
    {
        // Similar to GetMainLight(), but it takes a for-loop index. This figures out the
        // per-object light index and samples the light buffer accordingly to initialized the
        // Light struct. If ADDITIONAL_LIGHT_CALCULATE_SHADOWS is defined it will also compute shadows.
        int perObjectLightIndex = GetPerObjectLightIndex(i);
        Light light = GetAdditionalPerObjectLight(perObjectLightIndex, round(lightingData.positionWS * 2) / 2); // use original positionWS for lighting
        light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, shadowTestPosWS); // use offseted positionWS for shadow test

        // Different function used to shade additional lights.
        additionalLightSumResult += ShadeSingleLight(surfaceData, lightingData, light, true);
    }
#endif
    //==============================================================================================

    // emission
    //half3 emissionResult = ShadeEmission(surfaceData, lightingData);
    
    return CompositeAllLightResults(indirectResult, mainLightResult, additionalLightSumResult, 0, surfaceData, lightingData);
}

half3 ApplyFog(half3 color, Varyings input)
{
    half fogFactor = input.positionWSAndFogFactor.w;
    // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
    // with a custom one.
    color = MixFog(color, fogFactor);

    return color;
}

half4 frag(Varyings input) : SV_Target
{
    
    SpriteSurfaceData surfaceData = InitializeSurfaceData(input);

    SpriteLightingData lightingData = InitializeLightingData(input);
 
    // apply all lighting calculation
    half3 color = ShadeAllLights(surfaceData, lightingData);
    
    color = ApplyFog(color, input);
    
    return half4(color, surfaceData.alpha);
}

half4 frag_unlit(Varyings input) : SV_Target
{
    half4 baseColor = GetFinalBaseColor(input);
    half3 color = baseColor.rgb;
    half alpha = baseColor.a;
    
    color = ApplyFog(color, input);
    
    return half4(color, alpha);
}

void BaseColorAlphaClipTest(Varyings i)
{
    DoClipTestToTargetAlphaValue(tex2D(_MainTex, i.uv).a);
}