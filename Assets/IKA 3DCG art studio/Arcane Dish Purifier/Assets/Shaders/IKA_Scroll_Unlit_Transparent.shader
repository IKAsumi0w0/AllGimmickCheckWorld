Shader "IKA3D/IKA_Scroll_Unlit_Transparent"
{
    Properties{
        _MainTex("Primary Texture", 2D) = "white" {}
        _SecondaryTex("Secondary Texture", 2D) = "white" {}
        _TertiaryTex("Tertiary Texture", 2D) = "white" {} // 3枚目のテクスチャ
        _ScrollSpeedX("ScrollSpeedX", Range(-10.0, 10.0)) = 1.0
        _ScrollSpeedY("ScrollSpeedY", Range(-10.0, 10.0)) = 1.0
        [Toggle(InvertX)] _InvertX("InvertX", Float) = 0
        [Toggle(InvertY)] _InvertY("InvertY", Float) = 0
        _Color("Color", Color) = (1, 1, 1, 1)
        _Fade1("Fade between Primary and Secondary", Range(0, 1)) = 0.0 // 1枚目と2枚目のブレンド
        _Fade2("Fade between Secondary and Tertiary", Range(0, 1)) = 0.0 // 2枚目と3枚目のブレンド
        _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionStrength("Emission Strength", Range(0, 10)) = 1.0
    }
    SubShader{
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ InvertX
            #pragma shader_feature _ InvertY

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SecondaryTex;
            sampler2D _TertiaryTex; // 3枚目のテクスチャ
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            fixed4 _Color;
            float _Fade1; // 1枚目と2枚目のフェード
            float _Fade2; // 2枚目と3枚目のフェード
            fixed4 _EmissionColor;
            float _EmissionStrength;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // テクスチャ座標をスクロール
#if InvertX
                o.uv.x += _ScrollSpeedX * _Time.y;
#else
                o.uv.x -= _ScrollSpeedX * _Time.y;
#endif

#if InvertY
                o.uv.y += _ScrollSpeedY * _Time.y;
#else
                o.uv.y -= _ScrollSpeedY * _Time.y;
#endif

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1枚目と2枚目のフェード
                fixed4 color1 = tex2D(_MainTex, i.uv) * _Color;
                fixed4 color2 = tex2D(_SecondaryTex, i.uv) * _Color;
                fixed4 blended1 = lerp(color1, color2, _Fade1);

                // 2枚目と3枚目のフェード
                fixed4 color3 = tex2D(_TertiaryTex, i.uv) * _Color;
                fixed4 finalColor = lerp(blended1, color3, _Fade2);

                // エミッションカラーを適用
                fixed4 emission = _EmissionColor * _EmissionStrength;
                return finalColor + emission;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
