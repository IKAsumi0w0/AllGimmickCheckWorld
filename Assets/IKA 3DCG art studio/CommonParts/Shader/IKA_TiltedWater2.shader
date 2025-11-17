Shader "IKA3D/IKA_TiltedWater2"
{
    Properties
    {
        _FrontColor ("Color (Side)", Color) = (1,1,1,1)
        _BackColor ("Color (Surface)", Color) = (1,1,1,1)
        _Level ("Level", Range(0,1)) = 1
        _WaveHeight ("Wave Height", Float) = 0.03
        _UVScale ("UV Scale", Vector) = (2,2,0,0)
        _WaveSpeed ("Wave Speed", Float) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+0" "DisableBatching"="True" }
        LOD 100
        GrabPass { "_BackgroundTex" }
        Blend SrcAlpha OneMinusSrcAlpha

        // Pass 1: êÖñ Åió†ñ ÅjÅ{ã¸ê‹
        Pass
        {
            Cull Front
            ZWrite Off
            ZTest Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float _Level;
            float _WaveHeight;
            float2 _UVScale;
            float _WaveSpeed;
            fixed4 _BackColor;
            sampler2D _BackgroundTex;

            float2 rand(float2 st) {
                st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }

            float getWaveNoise(float2 st) {
                float2 uv = st * _UVScale + _Time.x * _WaveSpeed;
                float2 p = floor(uv);
                float2 f = frac(uv);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = dot(rand(p), f);
                float b = dot(rand(p + float2(1.0, 0.0)), f - float2(1.0, 0.0));
                float c = dot(rand(p + float2(0.0, 1.0)), f - float2(0.0, 1.0));
                float d = dot(rand(p + float2(1.0, 1.0)), f - float2(1.0, 1.0));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float getWaterSurfaceY()
            {
                float scaleY = length(unity_ObjectToWorld._m11_m21_m31);
                float bottomY = unity_ObjectToWorld._m13 - scaleY;
                return bottomY + _Level * scaleY * 2.0;
            }

            float3 calcWaveNormal(float2 posXZ) {
                float du = getWaveNoise(posXZ + float2(0.01, 0.0)) - getWaveNoise(posXZ - float2(0.01, 0.0));
                float dv = getWaveNoise(posXZ + float2(0.0, 0.01)) - getWaveNoise(posXZ - float2(0.0, 0.01));
                float3 normal = normalize(float3(-du, -dv, 1));
                normal = normalize(float3(normal.xy * _WaveHeight, normal.z));
                float3x3 rot = float3x3(1, 0, 0, 0, 0, 1, 0, -1, 0);
                return mul(rot, normal);
            }

            v2f vert(appdata v) {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = world.xyz;
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float surfaceY = getWaterSurfaceY();
                float wave = getWaveNoise(i.worldPos.xz) * _WaveHeight;
                clip((surfaceY + wave) - i.worldPos.y + 0.02);

                float3 normal = calcWaveNormal(i.worldPos.xz);

                float2 screenUV = i.vertex.xy / i.vertex.w;
                screenUV = screenUV * 0.5 + 0.5;
                screenUV += normal.xz * 0.03;

                fixed4 bg = tex2D(_BackgroundTex, screenUV);
                return lerp(bg, _BackColor, _BackColor.a);
            }
            ENDCG
        }

        // Pass 2: ë§ñ 
        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragSide
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _Level;
            float _WaveHeight;
            float2 _UVScale;
            float _WaveSpeed;
            fixed4 _FrontColor;

            float2 rand(float2 st) {
                st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
            }

            float getWaveNoise(float2 st) {
                float2 uv = st * _UVScale + _Time.x * _WaveSpeed;
                float2 p = floor(uv);
                float2 f = frac(uv);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = dot(rand(p), f);
                float b = dot(rand(p + float2(1.0, 0.0)), f - float2(1.0, 0.0));
                float c = dot(rand(p + float2(0.0, 1.0)), f - float2(0.0, 1.0));
                float d = dot(rand(p + float2(1.0, 1.0)), f - float2(1.0, 1.0));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float getWaterSurfaceY()
            {
                float scaleY = length(unity_ObjectToWorld._m11_m21_m31);
                float bottomY = unity_ObjectToWorld._m13 - scaleY;
                return bottomY + _Level * scaleY * 2.0;
            }

            v2f vert(appdata v) {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = world.xyz;
                return o;
            }

            fixed4 fragSide(v2f i) : SV_Target {
                float surfaceY = getWaterSurfaceY();
                float wave = getWaveNoise(i.worldPos.xz) * _WaveHeight;
                clip((surfaceY + wave) - i.worldPos.y);
                return _FrontColor;
            }
            ENDCG
        }
    }
}
