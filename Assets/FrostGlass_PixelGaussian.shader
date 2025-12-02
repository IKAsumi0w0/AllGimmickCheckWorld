Shader "IKA/FrostGlass_PixelGaussian"
{
    Properties
    {
        _TintColor   ("Tint Color (rgb) / Mix (a)", Color) = (1,1,1,0)
        _Radius      ("Gaussian Kernel Radius (samples)", Range(0,5)) = 3
        _Sigma       ("Gaussian Sigma", Range(0.5,8)) = 2.0
        _BlurScale   ("Blur Pixel Scale", Range(0,50)) = 10
        _Alpha       ("Alpha", Range(0,1)) = 0.9
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

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;

            float4 _TintColor;
            float  _Radius;
            float  _Sigma;
            float  _BlurScale;
            float  _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int r = (int)_Radius;            // サンプル数は半径5以内
                float sigma2 = _Sigma * _Sigma;

                // 1ピクセル分 × BlurScale で半径を広げられる
                float2 blurPx = _GrabTexture_TexelSize.xy * _BlurScale;

                fixed4 sum = 0;
                float wsum = 0;

                for (int x = -5; x <= 5; x++)
                {
                    if (abs(x) > r) continue;
                    for (int y = -5; y <= 5; y++)
                    {
                        if (abs(y) > r) continue;

                        float r2 = (float)(x*x + y*y);
                        float w = exp(-r2 / (2.0 * sigma2));

                        float4 uv = i.grabPos;
                        uv.xy += float2(blurPx.x * x, blurPx.y * y) * uv.w;

                        sum += tex2Dproj(_GrabTexture, uv) * w;
                        wsum += w;
                    }
                }

                fixed4 col = (wsum > 0) ? (sum / wsum) : tex2Dproj(_GrabTexture, i.grabPos);

                col.rgb = lerp(col.rgb, _TintColor.rgb, _TintColor.a);
                col.a = _Alpha;
                return col;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Diffuse"
}
