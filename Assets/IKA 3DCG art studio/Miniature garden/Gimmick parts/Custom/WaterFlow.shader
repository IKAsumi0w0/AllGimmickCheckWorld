Shader "Custom/WaterFlow" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 0.5)
        _Speed("Speed", Range(0, 10)) = 1

        _NormalMap("Normal map", 2D) = "bump" {}
        _Shininess("Shininess", Range(0.0, 1.0)) = 0.078125
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    half3 lightDir : TEXCOORD1;
                    half3 viewDir : TEXCOORD2;
                };

                float4 _LightColor0;
                sampler2D _MainTex;
                float4 _Color;
                float _Speed;
                float4 _MainTex_ST;
                sampler2D _NormalMap;
                half _Shininess;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float2 offset = float2(0, _Time.y);
                    o.uv = TRANSFORM_TEX(v.uv + (offset * _Speed), _MainTex);
                    TANGENT_SPACE_ROTATION;
                    o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                    o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    i.lightDir = normalize(i.lightDir);
                    i.viewDir = normalize(i.viewDir);
                    half3 halfDir = normalize(i.lightDir + i.viewDir);


                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    col.a = _Color.a;

                    half3 normal = UnpackNormal(tex2D(_NormalMap, i.uv));
                    half4 diff = saturate(dot(normal, i.lightDir)) * _LightColor0;
                    half3 spec = pow(max(0, dot(normal, halfDir)), _Shininess * 128.0) * _LightColor0.rgb * col.rgb;
                    fixed4 color;
                    color.rgb = col.rgb * diff + spec;
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
