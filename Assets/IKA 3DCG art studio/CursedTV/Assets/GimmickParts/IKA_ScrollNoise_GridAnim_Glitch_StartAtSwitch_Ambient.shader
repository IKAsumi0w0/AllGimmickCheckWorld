Shader "IKA3D/IKA_ScrollNoise_GridAnim_Glitch_StartAtSwitch_Ambient"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _Columns ("Columns (X frames)", Range(1,32)) = 2
        _Rows    ("Rows (Y frames)",    Range(1,32)) = 2

        // 手動フレーム制御
        [Toggle]_UseManualFrame ("Use Manual Frame", Float) = 1
        _ManualFrame ("Manual Frame Index", Range(0,255)) = 0

        // ==== 切替グリッチ（Switch Glitch） ====
        _GlitchDuration  ("Glitch Duration (sec)", Range(0.0, 2.0)) = 0.25
        _GlitchStrength  ("Glitch Strength", Range(0.0, 1.0)) = 0.6
        _GlitchFrequency ("Switch Jitter Frequency", Range(0.5, 20.0)) = 8.0
        _GlitchLines     ("Switch Scanline Splits", Range(8, 1024)) = 256
        _ChromaticOffset ("Switch Chromatic Offset", Range(0.0, 4.0)) = 0.5
        _GlitchStartTime ("Glitch Start Time (sec)", Float) = -9999

        // ==== 常時グリッチ（Ambient Glitch） ====
        [Toggle]_AmbientGlitch ("Ambient Glitch Enabled", Float) = 1
        _AmbientGlitchStrength ("Ambient Glitch Strength", Range(0.0, 1.0)) = 0.15
        _AmbientGlitchFrequency("Ambient Jitter Frequency", Range(0.1, 10.0)) = 3.0
        _AmbientGlitchLines    ("Ambient Scanline Splits", Range(8, 1024)) = 256
        _AmbientChroma         ("Ambient Chromatic (0..1)*SwitchChromatic", Range(0.0, 1.0)) = 0.2

        // 反転
        [Toggle]_InvertX("Flip U", Float) = 0
        [Toggle]_InvertY("Flip V", Float) = 0

        _Tint ("Tint", Color) = (1,1,1,1)
        _UVTiling ("UV Tiling (xy)",  Vector) = (1,1,0,0)
        _UVOffset ("UV Offset (xy)",  Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _Tint;

        float _Columns, _Rows;
        float _UseManualFrame, _ManualFrame;

        // Switch glitch
        float _GlitchDuration, _GlitchStrength, _GlitchFrequency, _GlitchLines, _ChromaticOffset, _GlitchStartTime;

        // Ambient glitch
        float _AmbientGlitch, _AmbientGlitchStrength, _AmbientGlitchFrequency, _AmbientGlitchLines, _AmbientChroma;

        // flips
        float _InvertX, _InvertY;

        // UV
        float4 _UVTiling, _UVOffset;

        struct Input { float2 uv_MainTex; };

        float hash21(float2 p){
            p = frac(p*float2(123.34,345.45));
            p += dot(p,p+34.345);
            return frac(p.x*p.y);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float t = _Time.y;

            // ====== Grid UV ======
            float2 uv = IN.uv_MainTex * _UVTiling.xy + _UVOffset.xy;

            int total = (int)max(_Columns*_Rows, 1.0);
            int frameIndex = (_UseManualFrame > 0.5)
                ? (int)clamp(_ManualFrame, 0.0, (float)total-1.0)
                : 0;

            int col = frameIndex % (int)_Columns;
            int row = frameIndex / (int)_Columns;
            if (_InvertY > 0.5) row = (int)(_Rows-1) - row;
            if (_InvertX > 0.5) col = (int)(_Columns-1) - col;

            float2 cellSize = float2(1.0/_Columns, 1.0/_Rows);
            float2 uvGrid = frac(uv * float2(_Columns, _Rows));
            if (_InvertX > 0.5) uvGrid.x = 1.0 - uvGrid.x;
            if (_InvertY > 0.5) uvGrid.y = 1.0 - uvGrid.y;

            float2 cellMin = float2(col, row) * cellSize;
            float2 baseUV  = cellMin + uvGrid * cellSize;

            // ====== Switch Glitch g (decay by seconds) ======
            float dur = max(_GlitchDuration, 1e-6);
            float dt  = t - _GlitchStartTime;
            float g   = 0.0;
            if (dt >= 0.0 && dt < dur) {
                float prg = saturate(dt / dur);
                g = (1.0 - prg) * (1.0 - prg) * _GlitchStrength;
            }

            // ====== Ambient Glitch ga (weak, always on) ======
            float ga = 0.0;
            if (_AmbientGlitch > 0.5)
            {
                float linesA   = max(_AmbientGlitchLines, 8.0);
                float scanIdA  = floor(uvGrid.y * linesA) + row * linesA;
                float rndA     = hash21(float2(scanIdA, scanIdA*1.91));
                float wobA     = 0.5 + 0.5 * sin((uvGrid.y + t) * _AmbientGlitchFrequency * 6.28318 + rndA*6.28318);
                ga = wobA * _AmbientGlitchStrength;
            }

            // ====== Combine & Distort ======
            float linesS  = max(_GlitchLines, 8.0);
            float scanIdS = floor(uvGrid.y * linesS) + row * linesS;
            float rndS1   = hash21(float2(scanIdS, scanIdS*1.37));
            float rndS2   = hash21(float2(scanIdS*0.97, 7.1));

            float jitterS = (rndS1 - 0.5) * 0.15 * g;
            float jitterA = (rndS1 - 0.5) * 0.05 * ga;
            float shearS  = (rndS2 - 0.5) * 0.05 * g;
            float shearA  = (rndS2 - 0.5) * 0.02 * ga;
            float wobbleS = sin((uvGrid.y + t) * _GlitchFrequency * 6.28318) * 0.03 * g;
            float wobbleA = sin((uvGrid.y + t) * _AmbientGlitchFrequency * 6.28318) * 0.01 * ga;

            float2 guv = baseUV;
            guv.x += jitterS + wobbleS + jitterA + wobbleA;
            guv.y += shearS  + shearA;
            guv = clamp(guv, cellMin, cellMin+cellSize);

            // 色ずれ（Switch + Ambient）
            float chromaSwitch  = (_ChromaticOffset / max(_Columns,1.0)) * g;
            float chromaAmbient = (_ChromaticOffset / max(_Columns,1.0)) * (_AmbientChroma * ga);
            float chroma = chromaSwitch + chromaAmbient;

            // ====== Sample ======
            float3 rgb;
            {
                float2 uR = guv + float2( chroma * 0.5, 0);
                float2 uG = guv;
                float2 uB = guv + float2(-chroma * 0.5, 0);

                fixed4 sR = tex2D(_MainTex, clamp(uR, cellMin, cellMin+cellSize));
                fixed4 sG = tex2D(_MainTex, uG);
                fixed4 sB = tex2D(_MainTex, clamp(uB, cellMin, cellMin+cellSize));

                rgb = float3(sR.r, sG.g, sB.b);
            }

            fixed4 c = fixed4(rgb, tex2D(_MainTex, guv).a) * _Tint;
            o.Albedo = c.rgb;
            o.Alpha  = c.a;
        }
        ENDCG
    }
    FallBack "Transparent"
}
