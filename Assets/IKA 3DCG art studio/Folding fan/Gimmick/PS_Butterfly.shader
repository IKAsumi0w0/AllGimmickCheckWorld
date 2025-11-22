Shader "IKA 3DCG art studio/PS_Butterfly"
{
    Properties
    {
        [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        [HDR]_MainColor("MainColor",Color) = (1,1,1,1)
        _FlapSpeed("Flap Speed", Range(0,20)) = 10
        _FlapIntensity("Flap Intensity", Range(0,2)) = 1
        _MoveSpeed("Move Speed", Range(0,5)) = 1
        _MoveIntensity("Move Intensity", Range(0,1)) = 0.2
        _RandomFlap("Random Flap", Range(1,2)) = 1
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Tansparent"
            }

            Pass
            {
                Cull off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag


                #include "UnityCG.cginc"

                struct appdata
                {
                    float2 uv : TEXCOORD0;
                    float3 center : TEXCOORD1;
                    float random : TEXCOORD2;
                    float3 velocity : TEXCOORD3;
                    float4 color : COLOR;
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainColor;
                float _FlapSpeed;
                float _FlapIntensity;
                float _MoveIntensity;
                float _MoveSpeed;
                float _RandomFlap;

                float rand(float2 co)
                {
                    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
                }

                float fbm(float x, float t)
                {
                    return sin(x + t) + 0.5 * sin(2.0 * x + t) + 0.25 * sin(4.0 * x + t);
                }

                v2f vert(appdata v)
                {
                    v2f o;
                    float3 local = v.vertex - v.center;
                    float randomFlap = lerp(_FlapSpeed / _RandomFlap, _FlapSpeed, rand(v.random));
                    float flap = (sin(_Time.w * randomFlap) + 0.5) * 0.5 * _FlapIntensity;
                    half c = cos(flap * sign(local.x));
                    half s = sin(flap * sign(local.x));
                    half2x2 rotateMatrix = half2x2(c, -s, s, c);
                    local.xy = mul(rotateMatrix, local.xy);
                    float3 forward = normalize(v.velocity);
                    float3 up = float3(0, 1, 0);
                    float3 right = normalize(cross(forward, up));
                    float3x3 mat = transpose(float3x3(right, up, forward));
                    v.vertex.xyz = mul(mat, local);
                    v.vertex.xyz += v.center;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float move = fbm(87034 * v.random, _Time.w * _MoveSpeed) * _MoveIntensity;
                    o.vertex.y += move;
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 col = tex2D(_MainTex, -i.uv);
                    col.rgb *= _MainColor.rgb;
                    col *= i.color;
                    clip(col.a - 0.01);
                    return col;
                }
                ENDCG
            }
        }
}