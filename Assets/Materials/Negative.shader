Shader "Custom/Negative"
{
    Properties
    {
        [Header(Atlas Setting)]
        _AtlasUv ("Atlas UV", Vector) = (0,0,1,1) // C#에서 전달받을 변수
        
        [Header(Dissolve Settings)]
        _Dissolve ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveDensity ("Dissolve Density", Range(1, 500)) = 25.0 // 디졸브 무늬 크기
        _BurnCol1 ("Burn Inner Color", Color) = (1, 0.8, 0.2, 1)
        _BurnCol2 ("Burn Outer Color", Color) = (1, 0.3, 0, 1)
        
        [Header(Negative Settings)]
        _InvertIntensity ("Invert Lightness", Range(0, 1)) = 1.0
        _HueOffset ("Hue Shift Offset", Range(0, 1)) = 0.2
        _Tint ("Base Tint Color", Color) = (0.309, 0.388, 0.403, 0.0)
        
        [Header(Shine Settings)]
        _ShineSpeed ("Shine Speed", Range(0, 5)) = 0.3
        _ShineIntensity ("Shine Intensity", Range(0, 2)) = 1.0
        _PatternScale ("Pattern Scale", Range(0.1, 10)) = 4.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AtlasUv, _BurnCol1, _BurnCol2;
            float _Dissolve, _DissolveDensity;
            float _InvertIntensity, _HueOffset;
            float4 _Tint;
            float _ShineSpeed, _ShineIntensity, _PatternScale;

            float4 dissolve(float4 tex, float2 localUV) {
                if (_Dissolve <= 0.001) return tex;

                float adj_dissolve = (_Dissolve * _Dissolve * (3.0 - 2.0 * _Dissolve)) * 1.02 - 0.01;
                float dt = _Time.y * 5.0 + 2003.0;
                float2 uv_diss = (localUV - 0.5) * _DissolveDensity;
                
                float2 d1 = uv_diss + float2(sin(-dt / 14.3), cos(-dt / 9.9));
                float2 d2 = uv_diss + float2(cos( dt / 5.3),  cos( dt / 6.1));
                float2 d3 = uv_diss + float2(sin(-dt / 8.7), sin(-dt / 4.9));

                float fieldD = (1.0 + (cos(length(d1) / 1.94) + sin(length(d2) / 3.31) * cos(d2.y / 1.57) + cos(length(d3) / 2.71) * sin(d3.x / 2.19))) / 2.0;
                float resD = (0.5 + 0.5 * cos((adj_dissolve * 0.1) + (fieldD - 0.5) * 3.14));

                // Burn Edge 효과
                float burnWidth = 0.05 * (0.5 - abs(adj_dissolve - 0.5));
                if (tex.a > 0.01 && resD < adj_dissolve + burnWidth * 2.0 && resD > adj_dissolve) {
                    float4 burnColor = (resD < adj_dissolve + burnWidth) ? _BurnCol1 : _BurnCol2;
                    return float4(burnColor.rgb, tex.a);
                }

                return (resD > adj_dissolve) ? tex : float4(0,0,0,0);
            }

            // --- HSL 유틸리티 ---
            float hueHelper(float s, float t, float h) {
                h = frac(h);
                float hs = h * 6.0;
                if (hs < 1.0) return (t - s) * hs + s;
                if (hs < 3.0) return t;
                if (hs < 4.0) return (t - s) * (4.0 - hs) + s;
                return s;
            }

            float4 RGBtoHSL(float4 c) {
                float low = min(c.r, min(c.g, c.b));
                float high = max(c.r, max(c.g, c.b));
                float delta = high - low;
                float sum = high + low;
                float4 hsl = float4(0, 0, 0.5 * sum, c.a);
                if (delta > 0.0) {
                    hsl.y = (hsl.z < 0.5) ? delta / sum : delta / (2.0 - sum);
                    if (high == c.r) hsl.x = (c.g - c.b) / delta;
                    else if (high == c.g) hsl.x = (c.b - c.r) / delta + 2.0;
                    else hsl.x = (c.r - c.g) / delta + 4.0;
                    hsl.x = frac(hsl.x / 6.0);
                }
                return hsl;
            }

            float4 HSLtoRGB(float4 c) {
                if (c.y < 0.0001) return float4(c.zzz, c.a);
                float t = (c.z < 0.5) ? c.y * c.z + c.z : -c.y * c.z + (c.y + c.z);
                float s = 2.0 * c.z - t;
                return float4(hueHelper(s, t, c.x + 1.0/3.0), hueHelper(s, t, c.x), hueHelper(s, t, c.x - 1.0/3.0), c.a);
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 tex = tex2D(_MainTex, i.uv);
                float2 localUV;
                localUV.x = (i.uv.x - _AtlasUv.x) / (_AtlasUv.z - _AtlasUv.x);
                localUV.y = (i.uv.y - _AtlasUv.y) / (_AtlasUv.w - _AtlasUv.y);
                float t = _Time.y * _ShineSpeed;
                float s = _PatternScale;

                // 1. [Inversion 로직] HSL 변환 및 네거티브 적용
                float4 hsl = RGBtoHSL(tex);
                if (_InvertIntensity > 0) {
                    hsl.z = lerp(hsl.z, 1.0 - hsl.z, _InvertIntensity);
                }
                hsl.x = frac(-hsl.x + _HueOffset);
                
                // 베이스 색상 결정 (반전된 HSL + 틴트)
                float3 baseRGB = HSLtoRGB(hsl).rgb + 0.8 * _Tint.rgb;

                // 2. [Shine 로직] 간섭 파동 계산
                float low = min(tex.r, min(tex.g, tex.b));
                float high = max(tex.r, max(tex.g, tex.b));
                float delta = high - low - 0.1;

                float fac  = 0.8 + 0.9 * sin(s * (11.*localUV.x + 4.32*localUV.y) + t*12. + cos(t*5.3 + localUV.y*4.2 - localUV.x*4.));
                float fac2 = 0.5 + 0.5 * sin(s * (8.*localUV.x + 2.32*localUV.y) + t*5. - cos(t*2.3 + localUV.x*8.2));
                float fac3 = 0.5 + 0.5 * sin(s * (10.*localUV.x + 5.32*localUV.y) + t*6.11 + sin(t*5.3 + localUV.y*3.2));
                float fac4 = 0.5 + 0.5 * sin(s * (3.*localUV.x + 2.32*localUV.y) + t*8.11 + sin(t*1.3 + localUV.y*11.2));
                float fac5 = sin(0.9 * 16. * localUV.x + 5.32*localUV.y + t*12. + cos(t*5.3 + localUV.y*4.2 - localUV.x*4.));

                float maxfac = 0.7 * max(max(fac, max(fac2, max(fac3, 0.0))) + (fac + fac2 + fac3 * fac4), 0.0);

                // 3. [통합] 반전된 베이스 위에 다크 샤인 효과 얹기
                float3 finalFoil;
                finalFoil.r = baseRGB.r - delta + delta * maxfac * (0.7 + fac5 * 0.27) - 0.1;
                finalFoil.g = baseRGB.g - delta + delta * maxfac * (0.7 - fac5 * 0.27) - 0.1;
                finalFoil.b = baseRGB.b - delta + delta * maxfac * 0.7 - 0.1;

                // 4. 강도 조절 및 알파 처리
                float3 combinedRGB = lerp(baseRGB, finalFoil, _ShineIntensity);
                
                float alpha = tex.a;
                if (alpha < 0.7) alpha /= 3.0; // 원본 코드의 알파 컷오프 유지

                float4 col = float4(saturate(combinedRGB), alpha);

                return dissolve(col, localUV);
            }
            ENDHLSL
        }
    }
}