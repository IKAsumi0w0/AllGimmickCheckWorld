Shader "IKA3D/IKA_Scroll_Unlit"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _ScrollSpeedX("ScrollSpeedX", Range(-10.0, 10.0)) = 1.0
        _ScrollSpeedY("ScrollSpeedY", Range(-10.0, 10.0)) = 1.0
        [Toggle(InvertX)] _InvertX("InvertX", Float) = 0
        [Toggle(InvertY)] _InvertY("InvertY", Float) = 0
        _Color("Color", Color) = (1, 1, 1, 1) // カラープロパティの追加
    }
    SubShader{
        Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ InvertX
            #pragma shader_feature _ InvertY

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            fixed4 _Color; // カラーパラメータの宣言

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
                // テクスチャカラーにカラーパラメータを掛け合わせて出力
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}
