Shader "Custom/Foil"
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

        _FoilTime ("Foil Time (R:Speed, G:Phase)", Vector) = (0.15, 1.0, 0, 0)
        _Intensity ("Effect Intensity", Range(0, 2)) = 1.0
        _ColorTint ("Foil Blue Tint", Color) = (0.9, 0.8, 1.2, 1.0)
        _CenterOffset ("Center Offset (X, Y)", Vector) = (0.5, 0.5, 0, 0)
        _PatternSize ("Pattern Size (Scale)", Range(10, 2000)) = 25.0
        _RayDistance ("Ray Central Gap", Range(0, 10)) = 5.0 // 숫자가 작을수록 중앙에 가깝게 붙음
        _RayDensity ("Ray Density", Range(5, 10)) = 5.     // 빛줄기 굵기/밀도
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
            float4 _FoilTime;
            float _Intensity;
            float4 _ColorTint;
            float2 _CenterOffset;
            float _PatternSize;
            float _RayDistance;
            float _RayDensity;

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
                
                // 1. 좌표 조정 (Aspect Ratio 반영)
                // float2 adjusted_uv = uv - 0.5;
                // float aspect = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
                // adjusted_uv.x *= aspect;

                // 1. [해결] 중앙 정렬 보정
                // _CenterOffset을 통해 수동 조절도 가능하게 했으며, 기본값은 (0.5, 0.5)입니다.
                float2 adjusted_uv = localUV - _CenterOffset;
                float aspect = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
                adjusted_uv.x *= aspect;

                // 2. 시간 변수 (foil.r, foil.g 역할)
                float f_r = _Time.y * _FoilTime.x;
                float f_g = _Time.y * _FoilTime.y;

                // 3. 간섭 패턴 계산 (fac ~ fac4)
                float lenSize = length(_PatternSize * adjusted_uv);
                float fac = clamp(2.0 * sin((lenSize + f_r * 2.0) + 3.0 * (1.0 + 0.8 * cos(length(113.11 * adjusted_uv) - f_r * 3.12))) - 1.0 - max(5.0 - lenSize, 0.0), 0.0, 1.0);
                
                // 2. [수정] 뻗어나가는 빛줄기 (Ray)
                float2 rotater = float2(cos(f_r * 0.12), sin(f_r * 0.35));
                float dist = length(adjusted_uv);
                // angle은 -1 ~ 1 사이의 값입니다.
                float angle = dot(rotater, adjusted_uv) / (length(rotater) * dist + 0.0001);
                
                // 이 수식에서 _RayDistance를 0에 가깝게 줄이면 무늬가 중앙 바로 옆까지 붙습니다.
                // _RayDensity를 조절하여 줄기의 굵기도 제어합니다.
                // 중앙 공백 처리
                float rayMask = max(_RayDistance - length(30.0 * adjusted_uv), 0.0);

                // _RayDensity가 높을수록 줄기가 가늘어지고 많아집니다.
                float rayPattern = angle * _RayDensity * (2.2 + 0.9 * sin(f_r * 1.65 + 0.2 * f_g));
                
                float fac2 = clamp(5.0 * cos(f_g * 0.3 + rayPattern) - 4.0 - rayMask, 0.0, 1.0);
                
                float fac3 = 0.3 * clamp(2.0 * sin(f_r * 5.0 + localUV.x * 3.0 + 3.0 * (1.0 + 0.5 * cos(f_r * 7.0))) - 1.0, -1.0, 1.0);
                float fac4 = 0.3 * clamp(2.0 * sin(f_r * 6.66 + localUV.y * 3.8 + 3.0 * (1.0 + 0.5 * cos(f_r * 3.41))) - 1.0, -1.0, 1.0);

                // 4. 최종 강도 합성
                float maxfac = max(max(fac, max(fac2, max(fac3, max(fac4, 0.0)))) + 2.2 * (fac + fac2 + fac3 + fac4), 0.0);
                
                // 5. 원본 대비(Delta) 기반 색상 변조
                float low = min(tex.r, min(tex.g, tex.b));
                float high = max(tex.r, max(tex.g, tex.b));
                float delta = min(high, max(0.5, 1.0 - low));

                float3 finalRGB = tex.rgb;
                finalRGB.r = tex.r - delta + delta * maxfac * 0.3;
                finalRGB.g = tex.g - delta + delta * maxfac * 0.3;
                finalRGB.b = tex.b + delta * maxfac * 1.9; // 강렬한 푸른 광택

                // 6. 최종 처리
                float4 col =  float4(finalRGB * _ColorTint.rgb * _Intensity, tex.a);

                return dissolve(col, localUV);
            }
            ENDHLSL
        }
    }
}