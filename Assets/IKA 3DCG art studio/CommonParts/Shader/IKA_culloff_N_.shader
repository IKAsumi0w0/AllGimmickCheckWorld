Shader "IKA3D/IKA_culloff_N_"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map"  , 2D) = "bump"{}
        _BumpScale("Normal Scale", Range(0, 1)) = 1.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Cutoff("Cutoff"      , Range(0, 1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Cutout" "Queue" = "Overlay" }
            LOD 200
                Cull off

                Pass{
              ZWrite Off
              ZTest Always
              ColorMask 0
            }

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
            };

            half _Glossiness;
            half _Metallic;
            sampler2D _BumpMap;
            half _BumpScale;
            fixed4 _Color;


            UNITY_INSTANCING_BUFFER_START(Props)

            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {

                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;

                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
                fixed4 n = tex2D(_BumpMap, IN.uv_MainTex);
                o.Normal = UnpackScaleNormal(n, _BumpScale);
            }
            ENDCG
        }
            FallBack "Diffuse"
}
