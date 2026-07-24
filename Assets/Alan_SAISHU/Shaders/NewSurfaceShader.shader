Shader "Custom/URP/DepthFade_TintPower"
{
    Properties
    {
        _BaseColor  ("Tint Color", Color) = (0,0,0,1)
        _TintPower  ("Brightness", Range(0,4)) = 1     // ⚠可调明度
        _DepthMin   ("Fade Start (m)", Float) = 0.2
        _DepthMax   ("Fade End   (m)", Float) = 3
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha    // 标准透明
        ZWrite Off
        Cull Off

        Pass
        {
            Name "DepthFade_TintPower"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes { float3 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _TintPower;
                float  _DepthMin;
                float  _DepthMax;
            CBUFFER_END

            Varyings Vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.screenPos  = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 Frag (Varyings IN) : SV_Target
            {
                float planeDepth = LinearEyeDepth(IN.positionCS.z, _ZBufferParams);
                float sceneDepth = LinearEyeDepth(
                                   SampleSceneDepth(IN.screenPos.xy / IN.screenPos.w), _ZBufferParams);
                float diff  = max(0, sceneDepth - planeDepth);
                float alpha = saturate((diff - _DepthMin) / (_DepthMax - _DepthMin));

                float3 tint = _BaseColor.rgb * _TintPower;   // 抬高亮度
                return float4(tint, alpha * _BaseColor.a);
            }
            ENDHLSL
        }
    }
    Fallback Off
}