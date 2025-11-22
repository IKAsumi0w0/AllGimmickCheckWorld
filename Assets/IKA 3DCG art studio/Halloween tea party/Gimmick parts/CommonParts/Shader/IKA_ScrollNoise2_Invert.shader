Shader "IKA3D/IKA_ScrollNoise2_Invert"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ScrollSpeedX("ScrollSpeedX", Range(0.0, 10.0)) = 1.0
        _ScrollSpeedY("ScrollSpeedY", Range(0.0, 10.0)) = 1.0
        [Toggle(InvertX)] _InvertX("InvertX", Float) = 0
        [Toggle(InvertY)] _InvertY("InvertY", Float) = 0
        _NoiseScale("Noise Scale", Float) = 10.0
        _Speed("Noise Speed", Float) = 1.0
        _Amount("Noise Amount", Float) = 0.05
        _JumpInterval("Jump Interval (Seconds)", Float) = 1.0
        _MaxMosaicSize("Max Mosaic Size", Range(1, 100)) = 10
        _MosaicDuration("Mosaic Duration (Seconds)", Range(0.0, 1.0)) = 0.5
        _NumberOfFrames("Number of Frames", Range(1, 10)) = 4 // サブテクスチャ数
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
        float _JumpInterval;
        float _MaxMosaicSize;
        float _MosaicDuration;
        int _NumberOfFrames;
        float _InvertX;
        float _InvertY;

        struct Input {
            float2 uv_MainTex;
        };

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

            // ノイズの適用
            float2 noiseUV = uv * _NoiseScale + _Time * _Speed;
            float noiseValue = noise(noiseUV);
            uv.x += (noiseValue - 0.5) * _Amount;

            // 時間に基づくフレーム切り替えのタイミングを取得
            float jumpTime = floor(_Time / _JumpInterval);
            float timeInInterval = frac(_Time / _JumpInterval);

            // フェードアウト用モザイクの計算
            float mosaicSize = 0.0;
            if (timeInInterval < _MosaicDuration)
            {
                mosaicSize = lerp(1.0, _MaxMosaicSize, timeInInterval / _MosaicDuration);
            }

            // モザイクを適用
            if (mosaicSize > 1.0)
            {
                float2 mosaicUV = floor(uv * mosaicSize) / mosaicSize;
                uv = mosaicUV;
            }

            // Y軸に沿って配置されたフレームの選択と表示
            float frameHeight = 1.0 / _NumberOfFrames;  // 各フレームの高さ
            
		    float currentFrame = floor(frac(-_Time / (_JumpInterval * _NumberOfFrames)) * _NumberOfFrames);
	
            // フレーム切り替えの方向を_InvertXと_InvertYで制御
            if (_InvertY > 0.5)
            {
                uv.y = (1.0 - uv.y) * _NumberOfFrames; // Y軸を反転
            }
            else
            {
                uv.y = uv.y * _NumberOfFrames;
            }
            uv.y = (uv.y - currentFrame) * frameHeight;


            // スクロール処理
            if (_InvertX > 0.5)
            {
                uv.x += _ScrollSpeedX * jumpTime; // X軸を反転してスクロール
            }
            else
            {
                uv.x -= _ScrollSpeedX * jumpTime;
            }

            if (_InvertY > 0.5)
            {
                uv.y += _ScrollSpeedY * jumpTime; // Y軸を反転してスクロール
            }
            else
            {
                uv.y -= _ScrollSpeedY * jumpTime;
            }

            // テクスチャをサンプリング
            fixed4 texColor = tex2D(_MainTex, uv);

            o.Albedo = texColor.rgb;
            o.Alpha = texColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
