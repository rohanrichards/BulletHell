Shader "Unlit/ShadowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowStrength ("Shadow Strength", Range(0.0,10.0)) = 2.0
        _ShadowFalloff ("Shadow Falloff", Range(0.0,100.0)) = 2.0
        _ShadowFade ("Shadow Fade", Range(0.0,10.0)) = 0.0
        _LightPower ("Light Power", Range(0.0,5.0)) = 2.0
        _LightAmbient ("Light Ambient", Range(0.0,1.0)) = 0.02

        _TextureBump ("Texture Bumpiness", Range(0.0,50.0)) = 2.0
        _TextureBumpShadow ("Texture Bumpiness Shadow", Range(0.0,10.0)) = 0.5

        _MaxShadowLights ("Maximum Shadow Casting Lights", Int) = 2
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            float _ShadowStrength;
            float _ShadowFalloff;
            float _ShadowFade;
            float _LightPower;
            float _LightAmbient;
            float _TextureBump;
            float _TextureBumpShadow;

            int _MaxShadowLights;

#define MAX_LIGHTS 64
#define FLOATS_PER_LIGHT 6
#define MAX_LIGHTS_FLOATS 384 // set to MAX_LIGHTS * FLOATS_PER_LIGHT

            int _LightCount = 0;
            float _Lights[MAX_LIGHTS_FLOATS];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

//////////////////////////////
// PBR calcs
	        #define PI 3.14159265359

            float to_srgbf(float val) {return val < 0.0031308 ? val*12.92 : 1.055 * pow(val, 1.0/2.4) - 0.055; }
            float3 to_srgb(float3 v) {return float3(to_srgbf(v.x), to_srgbf(v.y), to_srgbf(v.z)); }

            float DistributionGGX(float3 N, float3 H, float roughness) {
                float a      = roughness*roughness;
                float a2     = a*a;
                float NdotH  = max(dot(N, H), 0.0);
                float NdotH2 = NdotH*NdotH;
                
                float num   = a2;
                float denom = (NdotH2 * (a2 - 1.0) + 1.0);
                denom = PI * denom * denom;
                
                return num / denom;
            }

            float GeometrySchlickGGX(float NdotV, float roughness) {
                float r = (roughness + 1.0);
                float k = (r*r) / 8.0;
            
                float num   = NdotV;
                float denom = NdotV * (1.0 - k) + k;
                
                return num / denom;
            }

            float GeometrySmith(float3 N, float3 V, float3 L, float roughness) {
                float NdotV = max(dot(N, V), 0.0);
                float NdotL = max(dot(N, L), 0.0);
                float ggx2  = GeometrySchlickGGX(NdotV, roughness);
                float ggx1  = GeometrySchlickGGX(NdotL, roughness);
                
                return ggx1 * ggx2;
            }

            float3 fresnelSchlick(float cosTheta, float3 F0) {
                return F0 + (1.0 - F0) * pow(max(1.0 - cosTheta, 0.0), 5.0);
            }  


            float3 pbr(float3 lightDir, float3 radiance, float3 V, float3 N, float metallic, float roughness, float3 F0, float3 albedo) {
                // calculate per-light radiance
                float3 L = -lightDir;
                float3 H = normalize(V + L);
            
                // cook-torrance brdf
                float NDF = DistributionGGX(N, H, roughness);        
                float G   = GeometrySmith(N, V, L, roughness);      
                float3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);
            
                float3 kS = F;
                float3 kD = float3(1.0, 1.0, 1.0) - kS;
                kD *= 1.0 - metallic;
            
                float3 numerator    = NDF * G * F;
                float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
                float3 specular     = numerator / max(denominator, 0.001);  
            
                // add to outgoing radiance Lo
                float NdotL = max(dot(N, L), 0.0);                
                return (kD * albedo / PI + specular) * radiance * NdotL; 
            }

//////////////////////////////

            bool IsShadowed(fixed4 col) {
                float3 input_col = float3(col.xyz);
                input_col = floor(input_col * 255.0); // converts into 0-255 space
                float3 pass_col = floor(input_col * 0.5) * 2.0; // removes lower bit
                //bool is_shadowed = length(pass_col - input_col) > 0.0;
                float3 diff = abs(pass_col - input_col);
                bool is_shadowed = (diff.x > 0.0) || (diff.y > 0.0) || (diff.z > 0.0);
                return is_shadowed;
            }

            fixed4 QuantizedTex(float2 in_uv) {
                float2 uv = (floor(in_uv * _MainTex_TexelSize.zw) + 0.5) * _MainTex_TexelSize.xy;
                return tex2Dlod(_MainTex, float4(uv, 0, 0));
            }

            float ColGradient(fixed4 col_a, fixed4 col_b) {
                const float pow_lev = 2.0;
                float3 diff = pow(float3(col_b.xyz), pow_lev) - pow(float3(col_a.xyz), pow_lev);
                return diff.x + diff.y + diff.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //const int NUM_STEPS = 96;
                float aspect_ratio = _MainTex_TexelSize.y / _MainTex_TexelSize.x;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //bool is_shadowed = IsShadowed(col);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float3 light_result = float3(0.0, 0.0, 0.0);
                for(int light = 0; light < _LightCount; light++) {

                    //float2 light_pos = _LightPos[light].xy;
                    int light_ind = FLOATS_PER_LIGHT * light;
                    float2 light_pos = float2(_Lights[light_ind], _Lights[light_ind + 1]);
                    float light_rad = _Lights[light_ind + 2];
                    float3 light_col = float3(_Lights[light_ind + 3], _Lights[light_ind + 4], _Lights[light_ind + 5]);
                    float2 delta = (i.uv - light_pos);
                    delta.x *= aspect_ratio;
                    float delta_len = length(delta);

                    if (delta_len > light_rad) {
                        continue;
                    }
                    //const int NUM_STEPS = 96;
                    //const int NUM_STEPS = int(ceil(delta_len / 0.001));
                    const int NUM_STEPS = clamp(8, 96, int(ceil(delta_len / 0.001)));

                    //fixed4 lookup = QuantizedTex(lerp(light_pos, i.uv, 0.5));
                    //bool blocked = !IsShadowed(lookup);

                    float blocked = 0.0;

                    fixed4 previous_lookup = QuantizedTex(light_pos);
                    if(light < _MaxShadowLights) {
                        for(int x = 0; x < NUM_STEPS; x++) {
                            float along_frac = float(x) / float(NUM_STEPS);
                            float2 pos = lerp(light_pos, i.uv, along_frac);
                            fixed4 lookup = QuantizedTex(pos);
                            //float prev_diff = abs(ColGradient(lookup, previous_lookup)) * _TextureBumpShadow;
                            float prev_diff = 0.0;//abs(ColGradient(lookup, previous_lookup)) * _TextureBumpShadow;
                            //float block_val = IsShadowed(lookup) ? prev_diff : pow(along_frac, _ShadowFade);
                            blocked += IsShadowed(lookup) ? prev_diff : pow(along_frac, _ShadowFade);
                            previous_lookup = lookup;
                        }
                    }

                    float normal_frac = 1.0 - ColGradient(col, previous_lookup) * _TextureBump;
                    float block_frac = pow(1.0 - blocked / float(NUM_STEPS), _ShadowStrength);
                    //float block_frac = step(blocked, 1);

                    float light_str = pow((1.0 - smoothstep(0.0, light_rad, delta_len)) * block_frac, _ShadowFalloff);// * normal_frac;

                    float3 albedo = col.xyz;
                    float2 metallic_roughness = float2(0.0, 0.95);
                    float3 F0 = float3(0.04, 0.04, 0.04);
                    F0 = lerp(F0, albedo, metallic_roughness.x);
            		float3 pbr_res = pbr(float3(1.0, 0.0, 0.0), light_col * light_str, float3(-1.0, 0.0, 0.0), float3(-1.0, 0.0, 0.0), metallic_roughness.x, metallic_roughness.y, F0, col.xyz);
                    // float3 pbr_res = pbr(float3(delta_len, -1.0, 0.0), light_col * light_str * 5.0, float3(0.0, 1.0, 0.0), normalize(float3(-normal_frac, 1.0, 0.0)), metallic_roughness.x, metallic_roughness.y, F0, col.xyz);

                    light_result += pbr_res;
                    //light_result += light_col * light_str;
                }

                //light_result = lerp(float3(_LightAmbient, _LightAmbient, _LightAmbient), float3(_LightPower, _LightPower, _LightPower), light_result);
                //col.xyz *= light_result;
                light_result += _LightAmbient * col.xyz;

                col.xyz = light_result;
                col.w = 1.0;
                /*
                if(is_shadowed) {
                    col.xyz += fixed3(abs(sin(i.uv.x * 100.0)), abs(sin(i.uv.y * 100.0)), 0.0) * 0.02;
                } else {
                    col.xyz += fixed3(0.0, abs(sin(i.uv.x * 500.0)), abs(sin(i.uv.y * 500.0))) * 0.02;
                }*/
                //return fixed4(0.0, 0.0, 1.0, 1.0);
                return col;
            }
            ENDCG
        }
    }
}
