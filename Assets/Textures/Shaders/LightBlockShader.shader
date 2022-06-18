Shader "BulletHell/LightBlockShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                if(col.w <= 0.0) {
                    discard;
                }

                float3 out_col = float3(col.xyz);
                out_col = floor(out_col * 255.0); // converts into 0-255 space
                out_col = floor(out_col / 2.0) * 2.0 + float3(1.0, 1.0, 1.0); // ensures lower bit is set by removing it and adding it back
                out_col = out_col / 255.0; // converts back into 0-1 space
                col.xyz = fixed3(out_col);

                //col.z = 1.0;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                //if((col.x + col.y + col.z) > 0.01) {
                return col;
            }
            ENDCG
        }
    }
}
