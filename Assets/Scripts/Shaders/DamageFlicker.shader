Shader "Custom/DamageFlicker"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FlickerColor ("Flicker Color", Color) = (1,0,0,1)
        _FlickerAmount ("Flicker Amount", Range(0, 1)) = 0
        _FlickerSpeed ("Flicker Speed", Range(0, 20)) = 10
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _FlickerColor;
            float _FlickerAmount;
            float _FlickerSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Only flicker if _FlickerAmount is greater than 0
                if (_FlickerAmount > 0)
                {
                    // Create fast, sharp flicker effect
                    float time = _Time.y * _FlickerSpeed;
                    float flicker = step(0.5, frac(time)); // Creates sharp on/off flicker
                    
                    // Mix between original color and flicker color
                    fixed4 finalColor = lerp(col, _FlickerColor, _FlickerAmount * flicker);
                    return finalColor;
                }
                
                return col;
            }
            ENDCG
        }
    }
}