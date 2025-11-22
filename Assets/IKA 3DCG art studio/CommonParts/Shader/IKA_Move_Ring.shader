Shader "IKA3D/Move_Ring" {

    Properties
    {
        _RingColor("RingColor", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_RangeValue("Value", Range(0.0, 0.95)) = 0.75
    }
	SubShader
	{
		Tags { "RenderType"="Fade" "Queue"="Transparent"}
		LOD 200

		Pass
		{
  			ZWrite ON
  			ColorMask 0
		}

		CGPROGRAM
#pragma surface surf Standard alpha:auto
#pragma target 3.0
		
		fixed4 _RingColor;
		fixed4 _Color;
		float _RangeValue;
		static const float S_NUM1 = _RangeValue;
		static const float S_NUM2 = 0.95 - S_NUM1;

		struct Input {
		float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			float3 localPos = IN.worldPos -  mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
 			float dist = distance(fixed3(0, 0, 0), localPos);
			//float val = abs(sin(-dist * 10.0 - _Time * 100));
			float val = abs(sin(-(dist - (dist - dist * dist) * 0.75) * 10.0 - _Time * 50));
			if (dist < 0.1) {
				if (val > 0.95) {
				o.Albedo = _RingColor;
				o.Alpha = dist * 10;
				}
				else if (val > S_NUM1) {
					o.Albedo = _RingColor;
					o.Alpha = dist * 10 / 3;
				}
				else {
					o.Albedo = _Color;
					o.Alpha = _Color.w;			
				}
			}
			else {
				if (val > 0.95) {
				o.Albedo = _RingColor;
				o.Alpha = 1 - dist;
				}
				else if (val > S_NUM1) {
					o.Albedo = _RingColor;
					o.Alpha = 1 - dist;
				}
				else {
					o.Albedo = _Color;
					o.Alpha = _Color.w;			
				}
			}
		}
	ENDCG
	}
		FallBack "Diffuse"
}
