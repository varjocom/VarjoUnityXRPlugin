Shader "Varjo/Undistort"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            struct Intrinsics {
                float4 K;
                float2 Kr;
                float2 P;
                float2 F;
                float2 C;
                float aspectRatio;
            };

            float3 cvToGl(float3 cv) { return float3(cv.x, -cv.y, -cv.z); }

            float3 glToCv(float3 gl) { return float3(gl.x, -gl.y, -gl.z); }

            float2 cvToGl(float2 cv) { return float2(cv.x, 1.0f - cv.y); }

            float2 glToCv(float2 gl) { return float2(gl.x, 1.0f - gl.y); }

            Intrinsics cvToGl(Intrinsics intrinsics)
            {
                intrinsics.C = cvToGl(intrinsics.C);
                return intrinsics;
            }

            Intrinsics glToCv(Intrinsics intrinsics)
            {
                intrinsics.C = glToCv(intrinsics.C);
                return intrinsics;
            }

            float4 uvToClip(float2 uv) { return float4(2.0 * uv - float2(1.0f, 1.0f), 0.5f, 1.0f); }

            struct RectifiedCamera {
                float4x4 invRectificationMatrix;
                float4x4 invProjectionMatrix;

                float4 rectifiedViewToCamera(float4 rectifiedView) { return mul(invRectificationMatrix, rectifiedView); }

                float4 clipToRectifiedView(float4 clip) { return mul(invProjectionMatrix, clip); }

                float4 uvToRectifiedView(float2 uv) { return clipToRectifiedView(uvToClip(uv)); }

                float4 uvToCamera(float2 uv) { return rectifiedViewToCamera(uvToRectifiedView(uv)); }

                float3 uvToRectifiedViewSpaceDir(float2 uv) { return normalize(uvToRectifiedView(uv).xyz); }

                float3 uvToCameraDir(float2 uv) { return normalize(uvToCamera(uv).xyz); }
            };

            /**
             * Transform a direction to a UV coordinate for sampling a distorted texture.
             * @param cvCameraDir Normalized direction in CV camera space.
             * @param cvCameraIntrinsics OpenCV camera intrinsics.
             * @return Distorted D3D UV coordinate.
             */
            float2 getSampleCoordCv(in float3 cvCameraDir, in Intrinsics cvCameraIntrinsics, in float2 srcSize)
            {
                float4 K = cvCameraIntrinsics.K;
                float2 Kr = cvCameraIntrinsics.Kr;
                float2 P = cvCameraIntrinsics.P;
                float2 F = cvCameraIntrinsics.F;
                float2 C = cvCameraIntrinsics.C;

                float2 U = float2(0, 0);

                // Omnidir
                // Naming follows OpenCV reference implementation
                // https://github.com/opencv/opencv_contrib/blob/master/modules/ccalib/src/omnidir.cpp
                float2 V = cvCameraDir.xy / (cvCameraDir.z + K.w);

                float r2 = dot(V, V);
                float r4 = r2 * r2;

                float radialDistortion = dot(K.xy, float2(r2, r4));
                float Vxy2 = 2.0f * V.x * V.y;
                float2 tangentialDistortion = float2(P.y * (r2 + 2.0f * V.x * V.x) + P.x * Vxy2, P.x * (r2 + 2.0f * V.y * V.y) + P.y * Vxy2);

                float2 xyD = V * (1.0f + radialDistortion) + tangentialDistortion;
                U = xyD * F + C;

                // Skew
                U.x += K.z * xyD.y;

                float srcAspect = srcSize.x / srcSize.y;

                U += 0.5f / srcSize;
                U.y = ((U.y - 0.5f) / cvCameraIntrinsics.aspectRatio * srcAspect) + 0.5f;

                if (all(U > 0.0f) && all(U < 1.0f))
                    return U;
                else
                    return float2(-1.0f, 1.0f);
            }

            float2 getSampleCoord(in float3 cameraDir, in Intrinsics cameraIntrinsics, in float2 srcSize)
            {
                return cvToGl(getSampleCoordCv(glToCv(cameraDir), glToCv(cameraIntrinsics), srcSize));
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4x4 _InvProjectionMatrix;
            float4x4 _InvRectificationMatrix;

            float4 _K;
            float2 _Kr;
            float2 _P;
            float2 _F;
            float2 _C;
            float _AspectRatio;

            float4 frag(v2f i)
                : SV_Target
            {
                RectifiedCamera rectifiedCamera;
                rectifiedCamera.invProjectionMatrix = _InvProjectionMatrix;
                rectifiedCamera.invRectificationMatrix = _InvRectificationMatrix;

                Intrinsics intrinsics;
                intrinsics.K = _K;
                intrinsics.Kr = _Kr;
                intrinsics.P = _P;
                intrinsics.F = _F;
                intrinsics.C = _C;
                intrinsics.aspectRatio = _AspectRatio;

                const float3 cameraSpaceDir = rectifiedCamera.uvToCameraDir(i.uv);
                const float2 distortedUv = getSampleCoord(cameraSpaceDir, intrinsics, _MainTex_TexelSize.zw);
                return tex2D(_MainTex, distortedUv);
            }
            ENDCG
        }
    }
}
