Shader "IKA3D/IKA_Scroll"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_ScrollSpeedX("ScrollSpeedX", Range(0.0, 10.0)) = 1.0
		_ScrollSpeedY("ScrollSpeedY", Range(0.0, 10.0)) = 1.0
		[Toggle(InvertX)] _InvertX("InvertX", Float) = 0
		[Toggle(InvertY)] _InvertY("InvertY", Float) = 0
}
		SubShader{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
#pragma shader_feature _ InvertX
#pragma shader_feature _ InvertY

#include "UnityCG.cginc"

		sampler2D _MainTex;
		float _ScrollSpeedX;
		float _ScrollSpeedY;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed2 uv = IN.uv_MainTex;

#if InvertX
		uv.x += _ScrollSpeedX * _Time;
#else
		uv.x -= _ScrollSpeedX * _Time;
#endif
		
#if InvertY
		uv.y += _ScrollSpeedY * _Time;
#else
		uv.y -= _ScrollSpeedY * _Time;
#endif

		o.Albedo = tex2D(_MainTex, uv);
		o.Alpha = tex2D(_MainTex, uv);
	}
	ENDCG
	}
		FallBack "Diffuse"
}
