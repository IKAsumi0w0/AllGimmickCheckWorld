Shader "IKA3D/IKA_WaterSurface"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0.3, 0.5, 0.5)
        _WaveSpeed ("Wave Speed", Range(0.1, 5)) = 1
        _WaveStrength ("Wave Strength", Range(0.00, 0.2)) = 0.05
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalMapScrollSpeed ("Normal Map Scroll Speed", Vector) = (0.05, 0.05, 0, 0)
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 4
        _ReflectionColor ("Reflection Color", Color) = (0.8, 0.9, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NormalMap;
            float4 _BaseColor;
            float _WaveSpeed;
            float _WaveStrength;
            float4 _NormalMapScrollSpeed;
            float _FresnelPower;
            float4 _ReflectionColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvScroll : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldViewDir : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float time = _Time.y;
                float wave = sin(time * _WaveSpeed + v.vertex.x * 5 + v.vertex.z * 5) * _WaveStrength;
                float3 displaced = v.vertex.xyz + v.normal * wave;

                o.pos = UnityObjectToClipPos(float4(displaced, 1.0));
                o.uv = v.uv + wave;
                o.uvScroll = v.uv + (_NormalMapScrollSpeed.xy * time);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldViewDir = _WorldSpaceCameraPos - worldPos;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 normalTex = UnpackNormal(tex2D(_NormalMap, i.uvScroll));
                float3 normal = normalize(i.worldNormal + normalTex * 0.3);

                float3 viewDir = normalize(i.worldViewDir);
                float fresnel = pow(1 - saturate(dot(normal, viewDir)), _FresnelPower);
                float4 col = _BaseColor;

                col.rgb = lerp(col.rgb, _ReflectionColor.rgb, fresnel);
                col.a = _BaseColor.a;

                return col;
            }
            ENDCG
        }
    }
}
