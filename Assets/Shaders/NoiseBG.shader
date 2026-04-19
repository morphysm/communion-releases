Shader "NukedLunch/NoiseBG"
{
    Properties
    {
        _Speed       ("Speed",       Float)      = 0.008
        _StarDensity ("Star Density",Float)      = 20.0
        _Energy      ("Energy (E)",  Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Speed, _StarDensity, _Energy;

            float hash(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            float stars(float2 uv, float density)
            {
                float2 cell  = floor(uv * density);
                float2 local = frac(uv * density);
                float h = hash(cell);
                if (h > 0.985)
                {
                    float dist = length(local - float2(0.5, 0.5));
                    return smoothstep(0.12, 0.0, dist) * (h - 0.985) * 60.0;
                }
                return 0.0;
            }

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f     { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t      = _Time.y * _Speed;
                float2 uv    = i.uv + float2(t * 0.2, t * 0.1);

                float s1     = stars(uv, _StarDensity);
                float s2     = stars(uv + float2(0.4, 0.6), _StarDensity * 1.5);
                float field  = (s1 + s2) * 0.5;

                // Pure black base, only stars visible
                return fixed4(field, field, field, 1.0);
            }
            ENDCG
        }
    }
}