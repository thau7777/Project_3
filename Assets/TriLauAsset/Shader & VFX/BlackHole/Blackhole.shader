Shader "MAG/Blackhole"
{
    Properties
    {
        // --- Distortion & Hole ---
        _GlobalStrength ("Global Strength", Range(0, 1)) = 1
        _DistortionStrength ("Distortion Strength", Range(0, 3)) = 1.5
        _HoleSize ("Hole Size", Range(0, 1)) = 0.2
        _HoleEdgeSmoothness ("Hole Edge Smoothness", Range(0, 10)) = 4

        // --- Fresnel ---
        _FresnelColor ("Fresnel Color", Color) = (0.2,0.6,1,1) [HDR]
        _FresnelIntensity ("Fresnel Intensity", Range(0, 10)) = 1.2
        _FresnelPower ("Fresnel Power", Range(0.1, 8)) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _CameraOpaqueTexture;

            CBUFFER_START(UnityPerMaterial)
            float _GlobalStrength;
            float _DistortionStrength;
            float _HoleSize;
            float _HoleEdgeSmoothness;

            // Fresnel
            float4 _FresnelColor;
            float _FresnelIntensity;
            float _FresnelPower;
            float _pad0; // padding

            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 positionNDC : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz); 
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS.xyz);
                OUT.positionNDC = ComputeNormalizedDeviceCoordinates(IN.positionOS.xyz, UNITY_MATRIX_MVP);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // --- Normalize normal & view direction ---
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);

                // --- N dot V ---
                float NdotVSat = max(0.0, dot(normalWS, viewDirWS));

                // --- Hole Mask ---
                float invertedHoleSize = 1.0 - _HoleSize * _GlobalStrength;
                float NDotVSatDeriv = length(float2(abs(ddx(NdotVSat)), abs(ddy(NdotVSat))));
                float holeMask = 1.0 - smoothstep(
                    invertedHoleSize,
                    invertedHoleSize + NDotVSatDeriv * _HoleEdgeSmoothness,
                    NdotVSat
                );

                // --- Distortion ---
                float fresnelMask = 1.0 - NdotVSat; 
                float distortionAmount = 1.0 - pow(abs(fresnelMask), _DistortionStrength * _GlobalStrength);
                distortionAmount = pow(distortionAmount, 6.0);

                float2 posNDCRemap = -2.0 * IN.positionNDC + 1.0;
                float2 uvOffset = distortionAmount * posNDCRemap;
                float2 distortedUVs = IN.positionNDC + uvOffset;

                float3 distortedBG = tex2D(_CameraOpaqueTexture, distortedUVs).rgb;

                // --- Fresnel Effect (HDR) ---
                float fresnelTerm = pow(saturate(fresnelMask), _FresnelPower) * _FresnelIntensity;
                float rimLocalizer = (1.0 - holeMask);
                float rimFactor = fresnelTerm * rimLocalizer;
                float3 fresnelColorRGB = _FresnelColor.rgb;
                float3 emission = fresnelColorRGB * rimFactor;

                // --- Compose final color ---
                float3 finalColor = (holeMask * distortedBG) + emission;

                return float4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}
