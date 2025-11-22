Shader "IKA3D/IKA_ScrollNoise"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScrollSpeedX("ScrollSpeedX", Range(0.0, 10.0)) = 1.0
		_ScrollSpeedY("ScrollSpeedY", Range(0.0, 10.0)) = 1.0
		[Toggle(InvertX)] _InvertX("InvertX", Float) = 0
		[Toggle(InvertY)] _InvertY("InvertY", Float) = 0
		_NoiseScale ("Noise Scale", Float) = 10.0
        _Speed ("Noise Speed", Float) = 1.0
        _Amount ("Noise Amount", Float) = 0.05
	}
		SubShader
		{
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
		float _NoiseScale;
        float _Speed;
        float _Amount;

		struct Input {
			float2 uv_MainTex;
		};
	
		// PerlinÉmÉCÉYê∂ê¨ä÷êî
		float rand(float2 n)
		{
			return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
		}

		float noise(float2 p)
		{
			float2 ip = floor(p);
			float2 u = frac(p);
			u = u * u * (3.0 - 2.0 * u);

			float res = lerp(
				lerp(rand(ip), rand(ip + float2(1.0, 0.0)), u.x),
				lerp(rand(ip + float2(0.0, 1.0)), rand(ip + float2(1.0, 1.0)), u.x),
				u.y
			);
			return res * res;
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed2 uv = IN.uv_MainTex;
            float2 noiseUV = uv * _NoiseScale + _Time * _Speed;
            float noiseValue = noise(noiseUV);
            uv.x += (noiseValue - 0.5) * _Amount;
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
