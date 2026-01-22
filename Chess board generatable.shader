Shader "Serebrennikov/Generate/TriplanarCheckerboard"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _Scale ("Scale", Float) = 1.0
        _RotationX ("Rotation X (YZ plane)", Float) = 0.0
        _RotationY ("Rotation Y (XZ plane)", Float) = 0.0
        _RotationZ ("Rotation Z (XY plane)", Float) = 0.0
        _Normal ("Virtual Normal", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float4 _ColorA;
            float4 _ColorB;
            float  _Scale;
            float  _RotationX;
            float  _RotationY;
            float  _RotationZ;
            float3 _Normal;

            Varyings vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.uv = float2((id << 1) & 2, id & 2);
                o.pos = float4(o.uv * 2.0 - 1.0, 0, 1);
                return o;
            }

            float Checker(float2 uv)
            {
                uv = floor(uv);
                return fmod(abs(uv.x + uv.y), 2.0);
            }

            float2 RotateUV(float2 uv, float angleDeg)
            {
                float a = radians(angleDeg);
                float s = sin(a);
                float c = cos(a);
                return float2(
                    uv.x * c - uv.y * s,
                    uv.x * s + uv.y * c
                );
            }

            float4 SampleChecker(float2 uv)
            {
                return lerp(_ColorA, _ColorB, Checker(uv));
            }

            float4 frag(Varyings i) : SV_Target
            {
                // Reconstruct a virtual world position from UV
                float3 positionWS;
                positionWS.xy = (i.uv - 0.5) * _Scale;
                positionWS.z  = 0.0;

                float3 normalWS = normalize(_Normal);

                float3 weight = abs(normalWS);
                weight /= (weight.x + weight.y + weight.z);

                float2 uvX = RotateUV(positionWS.yz, _RotationX);
                float2 uvY = RotateUV(positionWS.xz, _RotationY);
                float2 uvZ = RotateUV(positionWS.xy, _RotationZ);

                float4 colorX = SampleChecker(uvX);
                float4 colorY = SampleChecker(uvY);
                float4 colorZ = SampleChecker(uvZ);

                return
                    colorX * weight.x +
                    colorY * weight.y +
                    colorZ * weight.z;
            }
            ENDHLSL
        }
    }
}
