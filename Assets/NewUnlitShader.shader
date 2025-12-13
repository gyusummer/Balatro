Shader "Custom/ScreenVortexEffect"
{
    Properties
    {
        // 🌀 소용돌이 강도를 제어할 변수 (extern float vortex_amt 에 해당)
        _VortexAmount ("Vortex Amount", Float) = 1.0
        // 화면을 덮을 텍스처 (스크린 공간 효과이므로 필요)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // 후처리처럼 사용하기 위해 Pass 설정을 단순화합니다.
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            // 후처리를 위해 Depth Test를 끄고, Blending 없이 덮어씌웁니다.
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert // 정점 셰이더 함수 이름
            #pragma fragment frag // 프래그먼트 셰이더 함수 이름
            #include "UnityCG.cginc" // Unity 기본 헬퍼 함수 포함

            // --- 2. 입력 변수 정의 ---
            struct appdata
            {
                float4 vertex : POSITION; // 정점 위치
                float2 uv : TEXCOORD0; // UV 좌표
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION; // 최종 클립 공간 위치
            };

            // Properties 블록에서 정의한 변수 선언
            sampler2D _MainTex;
            float _VortexAmount; // _VortexAmount는 ShaderToy 코드의 vortex_amt와 동일

            // Unity 내장 변수: 화면 해상도 (w, h)
            // float4 _ScreenParams; // .x: width, .y: height (선택적으로 사용)
            // _MainTex_TexelSize.zw 가 (w, h)를 나타냅니다.

            // --- 3. 정점 셰이더 (Vertex Shader) ---
            v2f vert (appdata v)
            {
                v2f o;
                
                // --- LÖVE 2D 변수 매핑 ---
                // vertex_position.xy: Unity의 v.vertex.xy (Local Space)
                // love_ScreenSize.xy: Unity의 _ScreenParams.xy 또는 _MainTex_TexelSize.zw
                // love_ScreenSize.xy/length(love_ScreenSize.xy): 단위 벡터화된 화면 크기 (정규화된 크기)
                
                float2 screen_res = _ScreenParams.xy; // 화면 해상도
                float screen_len = length(screen_res);

                // 1. 정규화된 UV 좌표 계산 (화면 중심을 (0, 0)으로, 대각선 길이를 1로 만듦)
                // v.vertex는 로컬 공간의 정점 위치 (보통 -1 ~ 1 사이의 값)
                // 후처리 쿼드는 보통 0~1 혹은 -1~1 로컬 좌표를 가집니다.
                // 그러나 원본 코드는 '픽셀 좌표'로 계산을 시작합니다.
                // LÖVE 2D의 vertex_position은 픽셀 좌표(0 ~ Width, 0 ~ Height)일 가능성이 높으므로,
                // 이를 Unity의 정점 처리 방식에 맞게 변환해야 합니다.

                // v.vertex.xy를 화면 픽셀 위치로 변환했다고 가정하고 진행합니다.
                // Unity의 정점 위치는 클립 공간으로 넘어가기 직전에 변환되므로, 
                // 우리는 로컬 공간 정점을 기준으로 변환해야 합니다.

                // 방법론: 로컬 공간 정점 위치 (v.vertex.xy)를 클립 공간(-1~1)으로 먼저 변환하고, 
                // 이를 다시 화면 공간 픽셀 좌표처럼 다루기 위해 정규화합니다.
                
                // 로컬 좌표 v.vertex.xy (보통 -1에서 1 사이)
                float2 uv = v.vertex.xy;
                
                // 원본 코드의 의도를 따라, 화면 크기를 기준으로 정규화된 좌표를 다시 계산합니다.
                // 💡 핵심: 정점을 화면 중심(0,0)을 기준으로 정규화된 좌표로 변환합니다.
                // Unity에서 후처리용 쿼드는 v.vertex.xy가 이미 클립 공간 좌표(-1~1)일 수 있으나,
                // 여기서는 LÖVE 2D의 좌표계를 따라 다시 계산합니다.
                
                // LÖVE 2D의 vertex_position이 (0 ~ love_ScreenSize.xy)라고 가정하고, 이를 클립 공간으로 역변환합니다.
                // Unity 환경에서는 v.vertex가 이미 로컬 공간 좌표입니다.
                
                // 1. v.vertex.xy를 -1 ~ 1 (화면 중심 기준)으로 변환합니다. (이미 Unity의 Quad는 이럴 가능성이 높음)
                // 2. 이 좌표를 화면의 정규화된 크기로 나눕니다.
                uv = v.vertex.xy; // 로컬 좌표 (후처리 쿼드이므로 대략 -1 ~ 1)

                // 2. 소용돌이 매개변수 계산
                float vortex_amt = _VortexAmount; // 외부 변수 매핑

                float effectRadius = 1.6 - 0.05 * vortex_amt;
                float effectAngle = 0.5 + 0.15 * vortex_amt;

                // 3. 화면 비율 보정 및 거리(길이) 계산
                // 화면 비율을 고려한 정규화된 길이(len)를 계산합니다.
                float aspect_ratio = screen_res.x / screen_res.y;
                float2 ratio_uv = uv * float2(aspect_ratio, 1.0); // 화면 비율 보정
                float len = length(ratio_uv);
                
                float radius = length(uv); // 보정되지 않은 반지름

                // 4. 각도 계산 및 비틀림 적용
                // atan2(y, x) 사용 (ShaderToy 변환 때와 동일한 이유)
                float angle = atan2(uv.y, uv.x) + effectAngle * smoothstep(effectRadius, 0.0, len);

                // 5. 새로운 정점 위치 계산
                // Polar -> Cartesian 변환을 통해 비틀린 좌표를 얻습니다.
                float2 new_uv;
                new_uv.x = radius * cos(angle);
                new_uv.y = radius * sin(angle);
                
                // 6. 최종 클립 공간 위치로 변환
                // Unity의 Object Space -> Clip Space 변환
                // 원본 코드는 화면 좌표계로 복구하는 과정이 있지만, Unity는 클립 공간(v.vertex가 이미 근접한)에서
                // 변형을 가한 후 UnityObjectToClipPos()를 사용해야 합니다.
                
                // 변형된 좌표 (new_uv)를 기존 정점 위치에 다시 대입합니다.
                // 이 변형이 클립 공간에 그대로 투영됩니다.
                v.vertex.xy = new_uv.xy; 

                // Unity의 표준 변환을 통해 클립 공간으로 최종 변환
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // UV 좌표는 변경 없이 프래그먼트 셰이더로 전달
                
                return o;
            }

            // --- 4. 프래그먼트 셰이더 (Fragment Shader) ---
            fixed4 frag (v2f i) : SV_Target
            {
                // 정점 셰이더에서 좌표를 왜곡했으므로, 
                // 프래그먼트 셰이더는 왜곡된 좌표에 해당하는 텍스처 색상만 샘플링하면 됩니다.
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}