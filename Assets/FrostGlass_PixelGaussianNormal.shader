Shader "IKA/FrostGlass_PixelGaussianNormal"
{
    Properties
    {
        _TintColor   ("Tint Color (rgb) / Mix (a)", Color) = (1,1,1,0)

        _Radius      ("Gaussian Kernel Radius (samples)", Range(0,5)) = 3
        _Sigma       ("Gaussian Sigma", Range(0.5,8)) = 2.0
        _BlurScale   ("Blur Pixel Scale", Range(0,50)) = 10

        _BumpMap     ("Normal Map", 2D) = "bump" {}
        _BumpScale   ("Normal Strength", Range(0,2)) = 1.0
        _NormalDistortPixels ("Normal Distortion (pixels)", Range(0,10)) = 2.0

        _Alpha       ("Alpha", Range(0,1)) = 0.9
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // 背景キャプチャ
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

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;

            float4 _TintColor;
            float  _Radius;
            float  _Sigma;
            float  _BlurScale;

            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float  _BumpScale;
            float  _NormalDistortPixels;

            float  _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;   // ノーマルマップ用UV
            };

            struct v2f
            {
                float4 pos     : SV_POSITION;
                float4 grabPos : TEXCOORD0;
                float2 uv      : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _BumpMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // =========================
                // ノーマルマップから歪みオフセット
                // =========================
                float3 n = UnpackNormal(tex2D(_BumpMap, i.uv));
                n.xy *= _BumpScale;

                // 法線から「何ピクセル揺らすか」→ スクリーンUVオフセットへ
                float2 normalPixelOfs = n.xy * _NormalDistortPixels;
                float2 normalUVOfs = normalPixelOfs * _GrabTexture_TexelSize.xy; // ピクセル → UV

                // =========================
                // ガウシアンブラー
                // =========================
                int r = (int)_Radius;                // サンプル半径（0〜5）
                float sigma2 = _Sigma * _Sigma;

                // ブラー範囲（1px × BlurScale）
                float2 blurPx = _GrabTexture_TexelSize.xy * _BlurScale;

                fixed4 sum = 0;
                float  wsum = 0;

                for (int x = -5; x <= 5; x++)
                {
                    if (abs(x) > r) continue;
                    for (int y = -5; y <= 5; y++)
                    {
                        if (abs(y) > r) continue;

                        float r2 = (float)(x*x + y*y);
                        float w = exp(-r2 / (2.0 * sigma2));

                        float4 uv = i.grabPos;

                        // ガウシアンによるオフセット（ピクセルベース）
                        float2 gaussOfs = float2(blurPx.x * x, blurPx.y * y);

                        // ノーマルによるスクリーンオフセットを追加
                        uv.xy += (gaussOfs + normalUVOfs) * uv.w;

                        sum  += tex2Dproj(_GrabTexture, uv) * w;
                        wsum += w;
                    }
                }

                fixed4 col;
                if (r == 0 || wsum <= 0.0)
                {
                    // 半径0なら元の画面＋ノーマル歪みだけ
                    float4 uv = i.grabPos;
                    uv.xy += normalUVOfs * uv.w;
                    col = tex2Dproj(_GrabTexture, uv);
                }
                else
                {
                    col = sum / wsum;
                }

                // TintColor で色味調整
                col.rgb = lerp(col.rgb, _TintColor.rgb, _TintColor.a);
                col.a   = _Alpha;

                return col;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Diffuse"
}
