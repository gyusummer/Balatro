Shader "Custom/Polychrome"
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

        [Header(Polychrome Settings)]
        _PoliScale ("Polychrome Density", Range(1, 500)) = 25.0 // 광택 촘촘함
        _Polychrome ("Shift (X) Speed (Y)", Vector) = (1.0, 1.0, 0, 0)
        _TimeSpeed ("Time Speed", Range(0, 5)) = 1.0
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
            float4 _AtlasUv; // C#에서 넘겨준 (minX, minY, maxX, maxY)
            float _Dissolve, _DissolveDensity, _TimeSpeed, _PoliScale;
            float4 _Polychrome, _BurnCol1, _BurnCol2;
            float _Hovering, _Distortion;

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

            // --- HSL/RGB 유틸리티 ---
            float hue(float s, float t, float h) {
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
                if (delta > 0) {
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
                return float4(hue(s, t, c.x + 1.0/3.0), hue(s, t, c.x), hue(s, t, c.x - 1.0/3.0), c.a);
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 tex = tex2D(_MainTex, i.uv);
                float2 localUV;
                localUV.x = (i.uv.x - _AtlasUv.x) / (_AtlasUv.z - _AtlasUv.x);
                localUV.y = (i.uv.y - _AtlasUv.y) / (_AtlasUv.w - _AtlasUv.y);
                
                // 1. Polychrome 광택 로직 (움직임 계산부)
                float low = min(tex.r, min(tex.g, tex.b));
                float high = max(tex.r, max(tex.g, tex.b));
                float delta = high - low;
                float saturation_fac = 1.0 - max(0.0, 0.05 * (1.1 - delta));

                float4 hsl = RGBtoHSL(float4(tex.r * saturation_fac, tex.g * saturation_fac, tex.b, tex.a));
                
                // [수정 포인트] pt 계산 방식을 더 직관적으로 변경
                // _Time.y는 유니티에서 제공하는 초 단위 시간입니다.
                float pt = _Time.y * _TimeSpeed * _Polychrome.y; 
                
                float2 uv_poli = (localUV - 0.5) * _PoliScale; 
                
                // 간섭 패턴의 속도를 각각 다르게 설정하여 더 역동적으로 움직이게 함
                float2 f1 = uv_poli + float2(sin(-pt * 0.7), cos(-pt * 0.9));
                float2 f2 = uv_poli + float2(cos( pt * 1.2),  cos( pt * 0.8));
                float2 f3 = uv_poli + float2(sin(-pt * 1.1), sin(-pt * 1.3));

                float fieldP = (1.0 + (cos(length(f1) / 1.948) + sin(length(f2) / 3.315) * cos(f2.y / 1.573) + cos(length(f3) / 2.719) * sin(f3.x / 2.192))) / 2.0;
                
                // 무지개 색상이 돌아가는 속도 결정
                float resP = (0.5 + 0.5 * cos(_Polychrome.x * 2.612 + (fieldP - 0.5) * 3.14));
                
                // 최종 색조(Hue)에 시간 흐름을 한 번 더 더해줌
                hsl.x = frac(hsl.x + resP + pt * 0.1); 
                hsl.y = min(0.6, hsl.y + 0.5);
                
                float3 colorEffect = HSLtoRGB(hsl).rgb;
                if (tex.a < 0.7) tex.a /= 3.0;

                // 최종 색상 결정
                float4 col = float4(colorEffect, tex.a);
                
                return dissolve(col, localUV);
            }
            ENDHLSL
        }
    }
}