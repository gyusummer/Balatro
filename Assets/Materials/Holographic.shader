Shader "Custom/Holographic"
{
    Properties
    {
        _MainTex ("Card Texture", 2D) = "white" {}
        _GlossSpeed ("Gloss Speed", Range(0, 200)) = 50
        _Intensity ("Holo Intensity", Range(0, 5)) = 2.0
        _GridSize ("Grid Size", Range(0.1, 20.0)) = 5
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
            float _GlossSpeed, _Intensity, _GridSize;
            float2 _HoloParams;

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
                float2 uv = i.uv;
                float4 tex = tex2D(_MainTex, uv);
                
                // 1. [Pixelation] 원본 코드의 floor uv 재현
                float2 res_size = _MainTex_TexelSize.zw; 
                float2 floored_uv = floor(uv * res_size) / res_size;
                
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
                    max(max(0.0, 7.0 * abs(cos(uv.x * grid * 20.0)) - 6.0),
                        max(0.0, 7.0 * cos(uv.y * grid * 45.0 + uv.x * grid * 20.0) - 6.0)),
                    max(0.0, 7.0 * cos(uv.y * grid * 45.0 - uv.x * grid * 20.0) - 6.0)
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
                float4 finalCol = lerp(tex, holoRGB, delta * _Intensity);

                // 투명도 처리 (원본 코드의 알파 컷오프 반영)
                if (finalCol.a < 0.7) finalCol.a /= 3.0;

                return finalCol;
            }
            ENDHLSL
        }
    }
}