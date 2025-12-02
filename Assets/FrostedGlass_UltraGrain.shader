Shader "IKA3D/IKA_FrostedGlass_UltraGrain"
{
    Properties
    {
        _MainTex ("Tint Texture", 2D) = "white" {}
        _TintColor ("Tint Color (rgb) / Mix (a)", Color) = (1,1,1,0)

        _Distortion ("Distortion", Range(0,0.1)) = 0.02
        _NoiseScale ("Wave Noise Scale", Range(1,100)) = 30

        _Blur ("Blur Amount", Range(0,3)) = 1.2

        _GrainScale ("Grain Scale", Range(20,10000)) = 220
        _GrainStrength ("Grain Strength", Range(0,1)) = 0.7
        _GrainContrast ("Grain Contrast", Range(1,10)) = 4.5

        _Alpha ("Alpha", Range(0,1)) = 0.75
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        GrabPass { "_GrabTexture" }

        Pass
        {
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // === Grab ===
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Distortion;
            float _NoiseScale;
            float _Blur;
            float4 _TintColor;
            float _Alpha;

            float _GrainScale;
            float _GrainStrength;
            float _GrainContrast;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
            };

            // --------------------------------------
            // 高精細ノイズ
            // --------------------------------------
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));

                float2 s = f*f*(3.0-2.0*f);
                return lerp(lerp(a, b, s.x), lerp(c, d, s.x), s.y);
            }

            // 3重合成ノイズ（細かい粒）
            float superNoise(float2 p)
            {
                float n  = noise(p);
                n += noise(p * 2.7) * 0.5;
                n += noise(p * 5.4) * 0.25;
                return n;
            }

            // 特殊コントラスト
            float HC(float x, float c)
            {
                x = pow(abs(x - 0.5) * 2, c) * 0.5;
                return saturate(x + 0.5);
            }

            // --------------------------------------
            // 頂点
            // --------------------------------------
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            // --------------------------------------
            // ピクセル
            // --------------------------------------
            fixed4 frag (v2f i) : SV_Target
            {
                // ---------- 高精細粒ノイズ ----------
                float2 p = i.uv * _GrainScale;

                float n = superNoise(p);
                float c = HC(n, _GrainContrast);  // メリハリ強化

                float grain = (c - 0.5) * 2.0 * _GrainStrength;

                // ---------- 歪み ----------
                float2 offset = float2(grain, -grain) * _Distortion;

                float4 grabUV = i.grabPos;
                grabUV.xy += offset * grabUV.w;

                fixed4 col0 = tex2Dproj(_GrabTexture, grabUV);

                // ---------- ブラー ----------
                float2 px = _GrabTexture_TexelSize.xy * _Blur;
                fixed4 col1 = tex2Dproj(_GrabTexture, grabUV + float4( px.x, 0, 0, 0));
                fixed4 col2 = tex2Dproj(_GrabTexture, grabUV + float4(-px.x, 0, 0, 0));
                fixed4 col3 = tex2Dproj(_GrabTexture, grabUV + float4(0,  px.y, 0, 0));
                fixed4 col4 = tex2Dproj(_GrabTexture, grabUV + float4(0, -px.y, 0, 0));

                fixed4 blurCol = (col0 + col1 + col2 + col3 + col4) / 5;

                fixed4 baseCol = lerp(col0, blurCol, 0.65);

                // ---------- Tint ----------
                fixed4 tint = tex2D(_MainTex, i.uv) * _TintColor;
                baseCol.rgb = lerp(baseCol.rgb, tint.rgb, _TintColor.a);

                // 粒の明暗を乗せる
                baseCol.rgb *= (1.0 + grain * 0.7);

                baseCol.a = _Alpha;
                return baseCol;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Diffuse"
}
