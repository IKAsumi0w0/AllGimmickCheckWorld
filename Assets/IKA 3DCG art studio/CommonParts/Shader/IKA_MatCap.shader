Shader "IKA3D/IKA_MatCap"
{
    Properties
    {
        _MainTex ("BaseTexture", 2D) = "white" {}
	    _MatcapTex ("MatcapTexture",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
		        float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
		        float3 viewNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
	        sampler2D _MatcapTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		        o.viewNormal = mul((float3x3)UNITY_MATRIX_V, UnityObjectToWorldNormal(v.normal)); //á@
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
		        float3 matCap = tex2D(_MatcapTex, i.viewNormal.xy * 0.5 + 0.5).rgb; //áAÅ@-1 ~ 1 Ç 0 ~ 1Ç÷
		        col.rgb *= matCap; //áB
                return col;
            }
            ENDCG
        }
    }
}
