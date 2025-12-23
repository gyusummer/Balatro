Shader "Custom/Foil"
{
    Properties
    {
        _MainTex ("Card Texture", 2D) = "white" {}
        _FoilTime ("Foil Time (R:Speed, G:Phase)", Vector) = (0.15, 1.0, 0, 0)
        _Intensity ("Effect Intensity", Range(0, 2)) = 1.0
        _ColorTint ("Foil Blue Tint", Color) = (0.9, 0.8, 1.2, 1.0)
        _CenterOffset ("Center Offset (X, Y)", Vector) = (0.5, 0.5, 0, 0)
        _PatternSize ("Pattern Size (Scale)", Range(10, 2000)) = 90.0
        _RayDistance ("Ray Central Gap", Range(0, 10)) = 2.0 // 숫자가 작을수록 중앙에 가깝게 붙음
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
            float4 _FoilTime;
            float _Intensity;
            float4 _ColorTint;
            float2 _CenterOffset;
            float _PatternSize;
            float _RayDistance;
            float _RayDensity; 

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                float4 tex = tex2D(_MainTex, uv);
                
                // 1. 좌표 조정 (Aspect Ratio 반영)
                // float2 adjusted_uv = uv - 0.5;
                // float aspect = _MainTex_TexelSize.z / _MainTex_TexelSize.w;
                // adjusted_uv.x *= aspect;

                // 1. [해결] 중앙 정렬 보정
                // _CenterOffset을 통해 수동 조절도 가능하게 했으며, 기본값은 (0.5, 0.5)입니다.
                float2 adjusted_uv = uv - _CenterOffset;
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
                
                float fac3 = 0.3 * clamp(2.0 * sin(f_r * 5.0 + uv.x * 3.0 + 3.0 * (1.0 + 0.5 * cos(f_r * 7.0))) - 1.0, -1.0, 1.0);
                float fac4 = 0.3 * clamp(2.0 * sin(f_r * 6.66 + uv.y * 3.8 + 3.0 * (1.0 + 0.5 * cos(f_r * 3.41))) - 1.0, -1.0, 1.0);

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

                // 6. 알파 처리
                //float finalA = min(tex.a, 0.3 * tex.a + 0.9 * min(0.5, maxfac * 0.1));
                float finalA = tex.a;

                return fixed4(finalRGB * _ColorTint.rgb * _Intensity, finalA);
            }
            ENDHLSL
        }
    }
}