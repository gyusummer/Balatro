Shader "Unlit/Background"
{
    Properties
    {
        _PixelSizeFactor ("Pixel Size Factor", Float) = 700
        _Color1 ("Color1", Color) = (1,0,0, 1)
        _Color2 ("Color2", Color) = (0,0,1, 1)
        _Color3 ("Color3", Color) = (0,0,0, 1)
        _Speed("Speed", Float) = 1
        _SpinAmount ("Spin Amount", Float) = 1
        _SpinEase ("Spin Ease", Float) = 1
        _Contrast ("Contrast", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _PixelSizeFactor;
            float _Speed;
            float _SpinAmount;
            float _SpinEase;
            float _Contrast;
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 effect( float4 colour_1, float4 colour_2, float4 colour_3, float2 screen_coords, float spin_amount, float contrast )
            {
                //Convert to UV coords (0-1) and floor for pixel effect
                float pixel_size = length(_ScreenParams.xy)/_PixelSizeFactor;
                float2 uv = (floor(screen_coords.xy*(1./pixel_size))*pixel_size - 0.5*_ScreenParams.xy)/length(_ScreenParams.xy);// - float2(0.12, 0.);
                float uv_len = length(uv);

                //Adding in a center swirl, changes with time. Only applies meaningfully if the 'spin amount' is a non-zero number
                float speed = (_SpinEase*0.2) + 302.2;
                float new_pixel_angle = (atan2(uv.y, uv.x)) + speed - _SpinEase*20.*(1.*spin_amount*uv_len + (1. - 1.*spin_amount));
                float2 mid = (_ScreenParams.xy/length(_ScreenParams.xy))/2.;
                uv = (float2((uv_len * cos(new_pixel_angle) + mid.x), (uv_len * sin(new_pixel_angle) + mid.y)) - mid);

	            //Now add the paint effect to the swirled UV
                uv *= 30.;
                speed = _Time.y*(_Speed);
	            float2 uv2 = float2(uv.x+uv.y, uv.x+uv.y);

                for(int i=0; i < 5; i++) {
		            uv2 += sin(max(uv.x, uv.y)) + uv;
		            uv  += 0.5*float2(cos(5.1123314 + 0.353*uv2.y + speed*0.131121),sin(uv2.x - 0.113*speed));
		            uv  -= 1.0*cos(uv.x + uv.y) - 1.0*sin(uv.x*0.711 - uv.y);
	            }

                //Make the paint amount range from 0 - 2
                float contrast_mod = (0.25*contrast + 0.5*spin_amount + 1.2);
	            float paint_res =min(2., max(0.,length(uv)*(0.035)*contrast_mod));
                float c1p = max(0.,1. - contrast_mod*abs(1.-paint_res));
                float c2p = max(0.,1. - contrast_mod*abs(paint_res));
                float c3p = 1. - min(1., c1p + c2p);

                float4 ret_col = (0.3/contrast)*colour_1 + (1. - 0.3/contrast)*(colour_1*c1p + colour_2*c2p + float4(c3p*colour_3.rgb, c3p*colour_1.a));

                return ret_col;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = effect(_Color1, _Color2, _Color3, i.vertex, _SpinAmount, _Contrast);
                return col;
            }

            
            ENDCG
        }
    }
}
