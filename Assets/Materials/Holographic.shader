Shader "Custom/Holographic"
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

        _GlossSpeed ("Gloss Speed", Range(0, 200)) = 50
        _Intensity ("Holo Intensity", Range(0, 5)) = 1.5
        _GridSize ("Grid Size", Range(0.1, 20.0)) = 1
        _HoloParams ("Holo X: Shift, Y: Speed", Vector) = (1.0, 1.0, 0, 0)
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
            float4 _MainTex_TexelSize;
            float4 _AtlasUv, _BurnCol1, _BurnCol2;
            float _Dissolve, _DissolveDensity;
            float _GlossSpeed, _Intensity, _GridSize;
            float2 _HoloParams;

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
            
            // --- HSL/RGB 유틸리티 함수 ---
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
                
                // 1. [Pixelation] 원본 코드의 floor uv 재현
                float2 res_size = _MainTex_TexelSize.zw; 
                float2 floored_uv = floor(localUV * res_size) / res_size;
                
                // 2. [HSL 변환] 원본 색상을 분석 (0.5 비율로 파란색 보정 포함)
                float4 hsl = RGBtoHSL(0.5 * tex + 0.5 * float4(0, 0, 1, tex.a));

                // 3. [Plasma Field] 복합 파동 계산
                float t = _Time.y * 7.221 + _Time.y * _GlossSpeed;
                float2 uv_scaled = (floored_uv - 0.5) * 250.0;
                
                float2 p1 = uv_scaled + 50.0 * float2(sin(-t / 143.634), cos(-t / 99.432));
                float2 p2 = uv_scaled + 50.0 * float2(cos( t / 53.153),  cos( t / 61.453));
                float2 p3 = uv_scaled + 50.0 * float2(sin(-t / 87.532), sin(-t / 49.000));

                float field = (1.0 + (cos(length(p1) / 19.483) + sin(length(p2) / 33.155) * cos(p2.y / 15.73) + cos(length(p3) / 27.193) * sin(p3.x / 21.92))) / 2.0;
                
                // res: 최종 홀로그램 파동 강도
                float res = 0.5 + 0.5 * cos(_HoloParams.x * 2.612 + (field - 0.5) * 3.14);

                // 4. [Grid] 격자무늬 계산
                float grid = _GridSize;
                float fac = 0.5 * max(
                    max(max(0.0, 7.0 * abs(cos(localUV.x * grid * 20.0)) - 6.0),
                        max(0.0, 7.0 * cos(localUV.y * grid * 45.0 + localUV.x * grid * 20.0) - 6.0)),
                    max(0.0, 7.0 * cos(localUV.y * grid * 45.0 - localUV.x * grid * 20.0) - 6.0)
                );

                // 5. [HSL Manipulation] 색상 회전 및 강화
                hsl.x = frac(hsl.x + res + fac); // Hue 회전
                hsl.y *= 1.3;                    // 채도 증가
                hsl.z = hsl.z * 0.6 + 0.4;       // 명도 보정

                // 6. [Final Blending] 원본 이미지와 홀로그램 합성
                float low = min(tex.r, min(tex.g, tex.b));
                float high = max(tex.r, max(tex.g, tex.b));
                float delta = 0.2 + 0.3 * (high - low) + 0.1 * high; // 디테일 마스크

                float4 holoRGB = HSLtoRGB(hsl) * float4(0.9, 0.8, 1.2, tex.a);
                float4 col = lerp(tex, holoRGB, delta * _Intensity);

                // 투명도 처리 (원본 코드의 알파 컷오프 반영)
                if (col.a < 0.7) col.a /= 3.0;

                return dissolve(col, localUV);
            }
            ENDHLSL
        }
    }
}