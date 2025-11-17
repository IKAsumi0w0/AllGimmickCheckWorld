Shader "IKA3D/IKA_Rotation" {
	Properties{
		_MainTex("Water Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows alpha
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed2 uv = IN.uv_MainTex;
		uv.x += 3* _Time;
		//uv.y += 0.4 * _Time;
		o.Albedo = tex2D(_MainTex, uv);
		o.Alpha = tex2D(_MainTex, uv);
	}
	ENDCG
	}
		FallBack "Diffuse"
}