Shader "IKA3D/IKA_Standard"
{
    Properties
    {
        _Albedo("Albedo", 2D) = "white" {}          // アルベド（基本色）
        _Metallic("Metallic", 2D) = "black" {}      // メタリックマップ
        _Roughness("Roughness", 2D) = "white" {}    // ラフネスマップ
        _NormalMap("Normal Map", 2D) = "bump" {}    // ノーマルマップ
        _RoughnessStrength("Roughness Strength", Range(0, 1)) = 1.0 // ラフネスの強度
        _MetallicStrength("Metallic Strength", Range(0, 2)) = 1.0   // メタリックの強度
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldTangent : TEXCOORD2;
                float3 worldBinormal : TEXCOORD3;
            };

            sampler2D _Albedo;
            sampler2D _Metallic;
            sampler2D _Roughness;
            sampler2D _NormalMap;
            float _RoughnessStrength;
            float _MetallicStrength;  // メタリックの強度

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // ワールド空間の法線、接線、バイノーマルを計算
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 各マップのテクスチャを取得
                fixed4 albedo = tex2D(_Albedo, i.uv);
                float metallic = tex2D(_Metallic, i.uv).r * _MetallicStrength;  // メタリック強度を適用
                float roughness = tex2D(_Roughness, i.uv).r * _RoughnessStrength;

                // ノーマルマップの適用
                float3 normalMap = UnpackNormal(tex2D(_NormalMap, i.uv));
                float3x3 TBN = float3x3(i.worldTangent, i.worldBinormal, i.worldNormal);
                float3 worldNormal = normalize(mul(normalMap, TBN));

                // 簡易的なライティング
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(dot(worldNormal, lightDir), 0.0);

                // スペキュラハイライト計算
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.vertex.xyz);
                float3 halfDir = normalize(lightDir + viewDir);
                float specular = pow(max(dot(worldNormal, halfDir), 0.0), (1.0 - roughness) * 100.0);

                // 最終カラー
                fixed3 color = albedo.rgb * NdotL + specular * metallic;

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
