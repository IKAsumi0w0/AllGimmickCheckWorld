Shader "IKA3D/IKA_BubbleShader_AlphaMaxOnly"
{
    Properties
    {
        _MainColor ("Bubble Tint", Color) = (1,1,1,0.4)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeWidth ("Edge Width", Range(0,1)) = 0.2
        _BubbleCount ("Bubble Count", Int) = 10
        _MaxSize ("Base Bubble Size", Float) = 0.1
        _SizeVariation ("Bubble Size Variation", Range(0, 1)) = 0.2
        _GrowthSpeed ("Growth Speed", Range(0.0, 5.0)) = 1.0
        _MinSizeRatio ("Min Growth Ratio", Range(0, 1)) = 0.3
        _RefractionStrength ("Refraction Strength", Range(0, 0.1)) = 0.03
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 4
        _LightColor ("Specular Light Color", Color) = (1,1,1,1)
        _LightIntensity ("Light Intensity", Float) = 1.0
        _FringeIntensity ("Fringe Brightness", Range(0, 1)) = 0.6
        _MaskTex ("Bubble Mask (Grayscale)", 2D) = "white" {}
        _UseLighting ("Enable Lighting", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        GrabPass { "_GrabTex" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _MainColor;
            fixed4 _EdgeColor;
            float _EdgeWidth;

            int _BubbleCount;
            float _MaxSize;
            float _SizeVariation;
            float _GrowthSpeed;
            float _MinSizeRatio;
            float _RefractionStrength;
            float _FresnelPower;
            fixed4 _LightColor;
            float _LightIntensity;
            float _FringeIntensity;
            float _UseLighting;

            sampler2D _GrabTex;
            sampler2D _MaskTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            float2 hash2(float2 p)
            {
                return frac(sin(float2(
                    dot(p, float2(127.1, 311.7)),
                    dot(p, float2(269.5, 183.3))
                )) * 43758.5453);
            }

            float renderBubble(
                float2 uv,
                float2 seed,
                float time,
                float baseSize,
                float sizeVariation,
                float growthSpeed,
                float minSizeRatio,
                out float2 centerOut,
                out float sizeOut)
            {
                float2 rnd = hash2(seed);
                float2 center = frac(rnd);

                float2 offset = float2(
                    sin(dot(seed, float2(12.9898, 78.233)) + floor(time)) * 0.05,
                    cos(dot(seed, float2(39.3468, 11.135)) + floor(time)) * 0.05
                );
                center = frac(center + offset);

                float sizeScale = lerp(1.0 - sizeVariation, 1.0 + sizeVariation, frac(rnd.y * 7.77));
                float maxSize = baseSize * sizeScale;

                float growth = frac(time * growthSpeed + rnd.x);
                growth = lerp(minSizeRatio, 1.0, growth);
                float size = growth * maxSize;

                float dist = distance(uv, center);
                float bubble = smoothstep(size, size * 0.9, dist);
                float burstThreshold = maxSize * 0.95;
                float alpha = step(dist, size) * step(size, burstThreshold);

                centerOut = center;
                sizeOut = size;
                return bubble * alpha;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y;

                fixed4 col = fixed4(0,0,0,0);
                float maxAlpha = 0.0;

                for (int j = 0; j < 64; j++) // © 64‚Ü‚Å‚È‚ç‘åä•vi_BubbleCount‚Å§Œäj
                {
                    if (j >= _BubbleCount) break;

                    float2 seed = float2(j, j * 7.3);
                    float2 center;
                    float size;

                    float bubble = renderBubble(uv, seed, t + j * 1.37, _MaxSize, _SizeVariation, _GrowthSpeed, _MinSizeRatio, center, size);

                    float maskVal = tex2D(_MaskTex, center).r;
                    if (maskVal < 0.5 || bubble <= 0.001) continue;

                    float2 dir = normalize(uv - center);
                    float distNorm = saturate(distance(uv, center) / size);
                    float fringe = smoothstep(0.98, 1.0, distNorm) * _FringeIntensity;

                    float2 offset = dir * _RefractionStrength * (1.0 - distNorm * distNorm);
                    float2 grabUV = i.grabPos.xy / i.grabPos.w + offset;
                    fixed4 refracted = tex2D(_GrabTex, grabUV);

                    float viewDot = 1.0 - distNorm;
                    float fresnel = pow(viewDot, _FresnelPower);

                    float3 normal = float3(dir, sqrt(1.0 - saturate(dot(dir, dir))));
                    float3 lightDir = normalize(float3(0.3, 0.5, 1.0));
                    float NdotL = saturate(dot(normal, lightDir));
                    float3 lighting = (_UseLighting > 0.5) ? (_LightColor.rgb * NdotL * _LightIntensity) : float3(0, 0, 0);

                    fixed4 bubbleCol = lerp(refracted, _MainColor, 0.5);
                    bubbleCol.rgb += fresnel * _MainColor.rgb;
                    bubbleCol.rgb += lighting;
                    bubbleCol.rgb += float3(1,1,1) * fringe;

                    float edgeRegion = 1.0 - _EdgeWidth;
                    float edgeAmount = saturate((distNorm - edgeRegion) / _EdgeWidth);
                    bubbleCol.rgb = lerp(bubbleCol.rgb, _EdgeColor.rgb, edgeAmount);
                    bubbleCol.a = lerp(bubbleCol.a, _EdgeColor.a, edgeAmount) + edgeAmount * 0.1;

                    bubbleCol.a *= bubble;

                    if (bubbleCol.a > maxAlpha)
                    {
                        col = bubbleCol;
                        maxAlpha = bubbleCol.a;
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
