Shader "IKA3D/IKA_culloff_N_A"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map"  , 2D) = "bump"{}
        _BumpScale("Normal Scale", Range(0, 1)) = 1.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Alpha("Alpha", Range(0,1)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _BumpScale;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Alpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Alpha;

            // 法線マップ適用
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)) * _BumpScale;

            // カットアウト処理
            clip(o.Alpha - 0.5);
        }
        ENDCG
    }

    FallBack "Diffuse"
}
