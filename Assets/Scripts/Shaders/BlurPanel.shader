Shader "UI/BlurPanel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0, 10)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                fixed4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _BlurSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Simple blur effect
                float blurAmount = _BlurSize * 0.01;
                
                col += tex2D(_MainTex, i.uv + float2(blurAmount, 0));
                col += tex2D(_MainTex, i.uv + float2(-blurAmount, 0));
                col += tex2D(_MainTex, i.uv + float2(0, blurAmount));
                col += tex2D(_MainTex, i.uv + float2(0, -blurAmount));
                col += tex2D(_MainTex, i.uv + float2(blurAmount, blurAmount));
                col += tex2D(_MainTex, i.uv + float2(-blurAmount, -blurAmount));
                col += tex2D(_MainTex, i.uv + float2(blurAmount, -blurAmount));
                col += tex2D(_MainTex, i.uv + float2(-blurAmount, blurAmount));
                
                col /= 9.0;
                col *= i.color;
                
                return col;
            }
            ENDCG
        }
    }
}