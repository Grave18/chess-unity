Shader "psx/unlit"
{
    Properties
    {
        [MainTexture] _MainTex("Base Texture (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // #include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct ToVert
            {
                float3 pos : POSITION;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct VertToFrag
            {
                half4  pos : SV_POSITION;
                half4  color : COLOR0;
                half4  colorFog : COLOR1;
                float2 uv_MainTex : TEXCOORD0;
                half3  normal : TEXCOORD1;
            };

            uniform float4 unity_FogStart;
            uniform float4 unity_FogEnd;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            // Vertex Shader
            VertToFrag vert(ToVert IN)
            {
                VertToFrag OUT;

                // Vertex snapping
                float4 snapToPixel = TransformObjectToHClip(IN.pos);
                float4 vertex = snapToPixel;
                vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                vertex.x = floor(160 * vertex.x) / 160;
                vertex.y = floor(120 * vertex.y) / 120;
                vertex.xyz *= snapToPixel.w;
                OUT.pos = vertex;

                // Vertex lighting
                // OUT.color = IN.color*UNITY_LIGHTMODEL_AMBIENT;;
                OUT.color = UNITY_LIGHTMODEL_AMBIENT;

                const float distance = length(mul(UNITY_MATRIX_MV, IN.pos));

                // Affine Texture Mapping
                // float4 affinePos = vertex;
                OUT.uv_MainTex = TRANSFORM_TEX(IN.uv_MainTex, _MainTex);
                OUT.uv_MainTex *= distance + (vertex.w * (UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;
                OUT.normal = distance + (vertex.w * (UNITY_LIGHTMODEL_AMBIENT.a * 8)) / distance / 2;

                // Fog
                const float4 fogColor = unity_FogColor;

                const float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
                OUT.normal.g = fogDensity;
                OUT.normal.b = 1;

                OUT.colorFog = fogColor;
                OUT.colorFog.a = clamp(fogDensity, 0, 1);

                // Cut out polygons
                if(distance > unity_FogStart.z + unity_FogColor.a * 255)
                {
                    OUT.pos.w = 0;
                }

                return OUT;
            }

            // Fragment Shader
            float4 frag(VertToFrag IN) : SV_TARGET// : COLOR
            {
                // half4 c = tex2D(_MainTex, IN.uv_MainTex / IN.normal.r)*IN.color;
                const float4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv_MainTex/IN.normal.r);
                float4 resultColor = textureColor * IN.colorFog.a;
                resultColor.rgb += IN.colorFog.rgb * (1 - IN.colorFog.a);

                return resultColor;
            }
            ENDHLSL
        }
    }
}