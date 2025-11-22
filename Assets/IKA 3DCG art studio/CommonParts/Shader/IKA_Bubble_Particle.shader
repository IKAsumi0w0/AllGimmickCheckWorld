Shader "IKA3D/IKA_Bubble_Particle_Advanced"
{
    Properties
    {
        _MainColor ("Bubble Base Color", Color) = (1,1,1,0.3)
        _EdgeColor ("Edge Highlight Color", Color) = (1,1,1,1)
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 5
        _Brightness ("Brightness", Range(0, 2)) = 1
        _EdgeIntensity ("Edge Intensity", Range(0, 2)) = 1
        _ReflectionPower ("Reflection Power", Range(0, 2)) = 0.5
        _MaskContrast ("Mask Contrast", Range(0.1, 5)) = 1
        _CubeMap ("Reflection Cubemap", Cube) = "_Skybox" {}
        _MainTex ("Bubble Mask (Grayscale)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainColor;
            float4 _EdgeColor;
            float4 _TintColor;
            float _FresnelPower;
            float _Brightness;
            float _EdgeIntensity;
            float _ReflectionPower;
            float _MaskContrast;
            UNITY_DECLARE_TEXCUBE(_CubeMap);

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);

                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), _FresnelPower);
                fixed4 tex = tex2D(_MainTex, i.uv);

                float mask = pow(tex.r, _MaskContrast);

                fixed4 reflection = UNITY_SAMPLE_TEXCUBE(_CubeMap, reflect(-viewDir, normal));

                float3 baseColor = _MainColor.rgb * _Brightness;
                float3 edgeColor = _EdgeColor.rgb * fresnel * _EdgeIntensity;
                float3 reflectColor = reflection.rgb * _ReflectionPower;

                float3 finalColor = baseColor + edgeColor + reflectColor;

                finalColor *= _TintColor.rgb;

                finalColor = saturate(finalColor);

                float alpha = mask * _MainColor.a;

                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
