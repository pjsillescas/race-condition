Shader "RaceCondition/Water_Lake"
{
    Properties
    {
        [Header(Colors)]
        _ShallowColor ("Shallow Color", Color) = (0.25, 0.80, 0.80, 0.6)
        _DeepColor ("Deep Color", Color) = (0.01, 0.05, 0.30, 0.8)
        _DepthOffset ("Depth Offset", Range(0, 5)) = 0.5

        [Header(Waves)]
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 0.8
        _WaveHeight ("Wave Height", Range(0, 0.5)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(0.1, 10)) = 2.0

        [Header(Specular)]
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecIntensity ("Specular Intensity", Range(0, 5)) = 1.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.85

        [Header(Fresnel)]
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3.0
        _FresnelIntensity ("Fresnel Intensity", Range(0, 2)) = 0.8

        [Header(Ripple)]
        _RippleScale ("Ripple Scale", Range(0.1, 10)) = 2.0
        _RippleStrength ("Ripple Strength", Range(0, 0.3)) = 0.04

        [Header(Alpha)]
        _Alpha ("Alpha", Range(0, 1)) = 0.85
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #define UNITY_PI 3.141592

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

            // #include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
            };

            half4 _ShallowColor;
            half4 _DeepColor;
            float _DepthOffset;

            float _WaveSpeed;
            float _WaveHeight;
            float _WaveFrequency;

            half4 _SpecColor;
            float _SpecIntensity;
            float _Smoothness;

            half4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;

            float _RippleScale;
            float _RippleStrength;

            float _Alpha;

            float3 GerstnerWave(float4 wave, float3 position, inout float3 tangent, inout float3 binormal)
            {
                float steepness = wave.z;
                float wavelength = wave.w;
                float k = 2 * UNITY_PI / wavelength;
                float c = sqrt(9.8 / k);
                float2 d = normalize(wave.xy);
                float f = k * (dot(d, position.xz) - c * _Time.y * _WaveSpeed);
                float a = steepness / k;

                tangent += float3(
                    -d.x * d.x * (steepness * sin(f)),
                    d.x * (steepness * cos(f)),
                    -d.x * d.y * (steepness * sin(f))
                );
                binormal += float3(
                    -d.x * d.y * (steepness * sin(f)),
                    d.y * (steepness * cos(f)),
                    -d.y * d.y * (steepness * sin(f))
                );

                return float3(
                    d.x * (a * cos(f)),
                    a * sin(f),
                    d.y * (a * cos(f))
                );
            }

            void WaveDisplacement(float3 position, inout float3 positionOS, inout float3 normalOS)
            {
                float3 pos = position;

                float3 tangent = float3(1, 0, 0);
                float3 binormal = float3(0, 0, 1);

                float4 wave1 = float4(1, 0, _WaveHeight * 2, 4);
                float4 wave2 = float4(0.7, 0.7, _WaveHeight * 1.5, 3);
                float4 wave3 = float4(-0.3, 0.95, _WaveHeight, 2.5);
                float4 wave4 = float4(0.9, -0.4, _WaveHeight * 0.8, 5);

                float3 displacement = 0;
                displacement += GerstnerWave(wave1, pos, tangent, binormal);
                displacement += GerstnerWave(wave2, pos, tangent, binormal);
                displacement += GerstnerWave(wave3, pos, tangent, binormal);
                displacement += GerstnerWave(wave4, pos, tangent, binormal);

                positionOS += displacement;

                float3 normal = normalize(cross(binormal, tangent));
                normalOS = normal;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 positionOS = input.positionOS.xyz;
                float3 normalOS = input.normalOS;

                WaveDisplacement(input.positionOS.xyz, positionOS, normalOS);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(normalOS);

                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.viewDirWS = GetCameraPositionWS() - positionInputs.positionWS;
                output.uv = input.uv;

                output.shadowCoord = GetShadowCoord(positionInputs);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 viewDir = normalize(input.viewDirWS);
                float3 normalWS = normalize(input.normalWS);

                float2 rippleUV = input.positionWS.xz * _RippleScale + _Time.y * _WaveSpeed * 0.1;
                float ripple = sin(rippleUV.x) * cos(rippleUV.y);
                normalWS += float3(ripple * _RippleStrength, 0, ripple * _RippleStrength);
                normalWS = normalize(normalWS);

                float NdotV = saturate(dot(normalWS, viewDir));
                float fresnel = pow(1 - NdotV, _FresnelPower) * _FresnelIntensity;

                float depthFactor = saturate((input.positionWS.y + _DepthOffset) * 0.5);
                half4 waterColor = lerp(_DeepColor, _ShallowColor, depthFactor);

                Light mainLight = GetMainLight(input.shadowCoord);
                float3 lightDir = normalize(mainLight.direction);
                float3 halfDir = normalize(lightDir + viewDir);

                float NdotH = saturate(dot(normalWS, halfDir));
                float spec = pow(NdotH, _Smoothness * 128) * _SpecIntensity;
                half4 specColor = half4(_SpecColor * spec * mainLight.color, 1);

                // half4 fresnelColor = _FresnelColor * fresnel * mainLight.color;
                half4 fresnelColor = half4(_FresnelColor * fresnel * mainLight.color, 1);

                half4 final = waterColor + specColor + fresnelColor;
                final.a = _Alpha;

                return final;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
