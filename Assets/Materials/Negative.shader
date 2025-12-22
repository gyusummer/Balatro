Shader "Unlit/Negative"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	_Color ("Color", Color) = (0,0,0,0)
        _Negative ("Negative", Vector) = (0,0,0,0)
        dissolve ("Dissolve", Float) = 0
        texture_details ("Texture Details", Vector) = (0,0,0,0)
        _ImageDetails ("Image Details", Vector) = (0,0,0,0)
        _Shadow ("Shadow", Integer) = 0
        burn_colour_1 ("burn_colour_1", Vector) = (0,0,0,0)
        burn_colour_2 ("burn_colour_1", Vector) = (0,0,0,0)
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityCG.glslinc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float2 _Negative;
            float4 _Color;
            float dissolve;
            //float time = _Time.y;
            float4 texture_details;
            float2 _ImageDetails;
            int _Shadow;
            float4 burn_colour_1;
            float4 burn_colour_2;
            
            float4 dissolve_mask(float4 tex, float2 texture_coords, float2 uv)
            {
                bool shadow = _Shadow > 0;
                if (dissolve < 0.001) {
                    return float4(shadow ? float3(0.,0.,0.) : tex.xyz, shadow ? tex.a*0.3: tex.a);
                }
            
                float adjusted_dissolve = (dissolve*dissolve*(3.-2.*dissolve))*1.02 - 0.01; //Adjusting 0.0-1.0 to fall to -0.1 - 1.1 scale so the mask does not pause at extreme values
            
            	float t = _Time.y * 10.0 + 2003.;
            	float2 floored_uv = (floor((uv*texture_details.ba)))/max(texture_details.b, texture_details.a);
                float2 uv_scaled_centered = (floored_uv - 0.5) * 2.3 * max(texture_details.b, texture_details.a);
            	
            	float2 field_part1 = uv_scaled_centered + 50.*float2(sin(-t / 143.6340), cos(-t / 99.4324));
            	float2 field_part2 = uv_scaled_centered + 50.*float2(cos( t / 53.1532),  cos( t / 61.4532));
            	float2 field_part3 = uv_scaled_centered + 50.*float2(sin(-t / 87.53218), sin(-t / 49.0000));
            
                float field = (1.+ (
                    cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
                    cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;
                float2 borders = float2(0.2, 0.8);
            
                float res = (.5 + .5* cos( (adjusted_dissolve) / 82.612 + ( field + -.5 ) *3.14))
                - (floored_uv.x > borders.y ? (floored_uv.x - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
                - (floored_uv.y > borders.y ? (floored_uv.y - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
                - (floored_uv.x < borders.x ? (borders.x - floored_uv.x)*(5. + 5.*dissolve) : 0.)*(dissolve)
                - (floored_uv.y < borders.x ? (borders.x - floored_uv.y)*(5. + 5.*dissolve) : 0.)*(dissolve);
            
                if (tex.a > 0.01 && burn_colour_1.a > 0.01 && !shadow && res < adjusted_dissolve + 0.8*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
                    if (!shadow && res < adjusted_dissolve + 0.5*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
                        tex.rgba = burn_colour_1.rgba;
                    } else if (burn_colour_2.a > 0.01) {
                        tex.rgba = burn_colour_2.rgba;
                    }
                }
            
                return float4(shadow ? float3(0.,0.,0.) : tex.xyz, res > adjusted_dissolve ? (shadow ? tex.a*0.3: tex.a) : .0);
            }
            
            float hue(float s, float t, float h)
            {
            	float hs = fmod(h, 1.)*6.;
            	if (hs < 1.) return (t-s) * hs + s;
            	if (hs < 3.) return t;
            	if (hs < 4.) return (t-s) * (4.-hs) + s;
            	return s;
            }
            
            float4 RGB(float4 c)
            {
            	if (c.y < 0.0001)
            		return float4(c.zzz, c.a);
            
            	float t = (c.z < .5) ? c.y*c.z + c.z : -c.y*c.z + (c.y+c.z);
            	float s = 2.0 * c.z - t;
            	return float4(hue(s,t,c.x + 1./3.), hue(s,t,c.x), hue(s,t,c.x - 1./3.), c.w);
            }
            
            float4 HSL(float4 c)
            {
            	float low = min(c.r, min(c.g, c.b));
            	float high = max(c.r, max(c.g, c.b));
            	float delta = high - low;
            	float sum = high+low;
            
            	float4 hsl = float4(.0, .0, .5 * sum, c.a);
            	if (delta == .0)
            		return hsl;
            
            	hsl.y = (hsl.z < .5) ? delta / sum : delta / (2.0 - sum);
            
            	if (high == c.r)
            		hsl.x = (c.g - c.b) / delta;
            	else if (high == c.g)
            		hsl.x = (c.b - c.r) / delta + 2.0;
            	else
            		hsl.x = (c.r - c.g) / delta + 4.0;
            
            	hsl.x = fmod(hsl.x / 6., 1.);
            	return hsl;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);
            	float2 uv = (((i.uv)*(_ImageDetails)) - texture_details.xy*texture_details.ba)/texture_details.ba;
            
                float4 SAT = HSL(tex);
            
            	if (_Negative.g > 0.0 || _Negative.g < 0.0) {
            		SAT.b = (1.-SAT.b);
            	}
            	SAT.r = -SAT.r+0.2;
            
                tex = RGB(SAT) + _Color;//0.8*float4(79./255., 99./255.,103./255.,0.);
            
            	if (tex[3] < 0.7)
            		tex[3] = tex[3]/3.;
            	return dissolve_mask(tex*i.color, i.uv, uv);
            }
            ENDCG
        }
    }
}
