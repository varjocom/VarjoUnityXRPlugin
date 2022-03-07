Shader "Skybox/VSTBackground"
{
    Properties
    {
        _LeftTex("Texture", 2D) = "white" {}
        _RightTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags{"Queue" =
                 "Background"
                 "RenderType" =
                     "Background"
                     "PreviewType" = "Skybox"} Cull Off ZWrite Off

            CGINCLUDE
#include "UnityCG.cginc"
        uniform float4x4 _LeftProjection;
        uniform float4x4 _RightProjection;

        uniform float _GrayScale;

        struct appdata_t {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float4 posproj : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata_t v)
        {
            v2f o;

            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.posproj = mul(UNITY_MATRIX_MV, v.vertex);
            o.vertex = mul(UNITY_MATRIX_P, o.posproj);

            if (unity_StereoEyeIndex == 0) {
                o.posproj = mul(_LeftProjection, o.posproj);
            } else {
                o.posproj = mul(_RightProjection, o.posproj);
            }

            o.posproj = ComputeScreenPos(o.posproj);
            return o;
        }

        ENDCG

        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

            sampler2D _LeftTex;
            sampler2D _RightTex;

            half4 frag(v2f i)
                : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                i.posproj /= i.posproj.w;
                i.posproj.y = 1 - i.posproj.y;
                float4 col;
                if (unity_StereoEyeIndex == 0)
                {
                    col = tex2Dproj(_LeftTex, i.posproj);
                }
                else
                {
                    col = tex2Dproj(_RightTex, i.posproj);
                }

                return float4(col.r, lerp(col.g, col.r, _GrayScale), lerp(col.b, col.r, _GrayScale), 1);
            }
            ENDCG
        }
    }
}
