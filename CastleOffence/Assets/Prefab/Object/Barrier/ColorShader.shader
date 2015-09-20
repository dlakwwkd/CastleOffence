Shader "Custom/ColorShader"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
    }

    Category
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma fragmentoption ARB_fog_exp2
                
                #include "UnityCG.cginc"
        
                sampler2D _MainTex;
                float4 _TintColor;
        
                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };
    
                struct v2f
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };
        
                float4 _MainTex_ST;
        
                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    return o;
                }
        
                half4 frag(v2f i) : COLOR
                {
                    return i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                }
                ENDCG
            }
        }
    }
}
