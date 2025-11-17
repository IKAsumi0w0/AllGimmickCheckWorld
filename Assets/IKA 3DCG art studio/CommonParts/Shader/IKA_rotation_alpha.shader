Shader "IKA3D/IKA_Rotation_alpha" {
	Properties{
		_MainTex("Water Texture", 2D) = "white" {}
		_Cutoff("Cutoff"      , Range(0, 1)) = 0.5
	}
		SubShader{
		Tags{
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}
		LOD 200

		Cull Off

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff
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