Shader "Unlit/ShadowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowStrength ("Shadow Strength", Range(0.0,10.0)) = 2.0
        _ShadowRadius ("Shadow Radius", Range(0.0,5.0)) = 0.8
        _ShadowFalloff ("Shadow Falloff", Range(0.0,50.0)) = 2.0
        _LightPower ("Light Power", Range(0.0,5.0)) = 2.0
        _LightAmbient ("Light Ambient", Range(0.0,1.0)) = 0.02
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
            float _ShadowRadius;
            float _ShadowFalloff;
            float _LightPower;
            float _LightAmbient;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            bool IsShadowed(fixed4 col) {
                float3 input_col = float3(col.xyz);
                input_col = floor(input_col * 255.0); // converts into 0-255 space
                float3 pass_col = floor(input_col / 2.0) * 2.0; // removes lower bit
                bool is_shadowed = length(pass_col - input_col) > 0.0;
                return is_shadowed;
            }

            fixed4 QuantizedTex(float2 in_uv) {
                float2 uv = (floor(in_uv * _MainTex_TexelSize.zw) + 0.5) * _MainTex_TexelSize.xy;
                return tex2Dlod(_MainTex, float4(uv, 0, 0));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //bool is_shadowed = IsShadowed(col);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float2 light_pos = float2(0.5, 0.5);

                float light_result = 0.0;
                {
                    const int NUM_STEPS = 128;
                    //const float uv_step = 0.01;
                    float2 delta = (i.uv - light_pos);
                    float delta_len = length(delta);
                    //delta *= 1.0 / delta_len;

                    //fixed4 lookup = QuantizedTex(lerp(light_pos, i.uv, 0.5));
                    //bool blocked = !IsShadowed(lookup);

                    //bool blocked = false;
                    float blocked = 0.0;

                    fixed4 previous_lookup = QuantizedTex(light_pos);

                    for(int x = 0; x < NUM_STEPS; x++) {
                        float2 pos = lerp(light_pos, i.uv, float(x) / float(NUM_STEPS));
                        fixed4 lookup = QuantizedTex(pos);
                        float3 prev_delta = lookup.xyz - previous_lookup.xyz;
                        float prev_diff = pow(dot(prev_delta, prev_delta), 0.2) * 0.3;
                        blocked += IsShadowed(lookup) ? prev_diff : 1.0;
                        //blocked += IsShadowed(lookup) ? (pow((lookup.x + lookup.y + lookup.z) / 3.0, 10.1) * 0.5) : 1.0;
                        previous_lookup = lookup;
                    }
                    
                    float block_frac = pow(1.0 - blocked / float(NUM_STEPS), _ShadowStrength);
                    //float block_frac = step(blocked, 1);
                    light_result += (1.0 - smoothstep(0.0, _ShadowRadius, delta_len)) * block_frac;
                }

                light_result = lerp(_LightAmbient, _LightPower, pow(light_result, _ShadowFalloff));

                col.xyz *= light_result;
                /*
                if(is_shadowed) {
                    col.xyz += fixed3(abs(sin(i.uv.x * 100.0)), abs(sin(i.uv.y * 100.0)), 0.0) * 0.02;
                } else {
                    col.xyz += fixed3(0.0, abs(sin(i.uv.x * 500.0)), abs(sin(i.uv.y * 500.0))) * 0.02;
                }*/

                return col;
            }
            ENDCG
        }
    }
}
