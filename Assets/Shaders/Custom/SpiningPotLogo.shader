Shader "Custom/SpiningPotLogo"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        _LogoTex("Logo Texture", 2D) = "white" {}
        _LogoColor("Logo Color", Color) = (1, 0, 0, 1)

        _LogoCenter("Logo Center (UV)", Vector) = (0.5, 0.22, 0, 0)
        _LogoSize("Logo Size (UV)", Vector) = (0.6, 0.3, 0, 0)

        _LogoSpinTurnsPerSecond("Logo Spin (turns/sec)", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 positionWS  : TEXCOORD2;
            };

            TEXTURE2D(_LogoTex);
            SAMPLER(sampler_LogoTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _LogoColor;
                float4 _LogoCenter;
                float4 _LogoSize;
                float _LogoSpinTurnsPerSecond;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 baseRgb = _BaseColor.rgb;

                float2 halfSize = _LogoSize.xy * 0.5;
                float2 d = abs(IN.uv - _LogoCenter.xy);
                float rectMask = step(d.x, halfSize.x) * step(d.y, halfSize.y);

                float2 safeSize = max(_LogoSize.xy, float2(1e-5, 1e-5));
                float2 logoUV = (IN.uv - (_LogoCenter.xy - halfSize)) / safeSize;

                logoUV.x = frac(logoUV.x + _Time.y * _LogoSpinTurnsPerSecond);

                half logoMask = SAMPLE_TEXTURE2D(_LogoTex, sampler_LogoTex, logoUV).r;
                half m = saturate(logoMask) * rectMask;

                half3 albedo = lerp(baseRgb, _LogoColor.rgb, m);

                Light mainLight = GetMainLight();
                half3 normalWS = normalize(IN.normalWS);

                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 litColor = albedo * (mainLight.color * NdotL);

                return half4(litColor, 1.0);
            }
            ENDHLSL
        }
    }
}
