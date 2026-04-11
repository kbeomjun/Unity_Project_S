Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _OutlineColor;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // 외곽선 계산
                float outline = 0;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                outline += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                outline = saturate(outline);

                // 원본 + 외곽선 합성
                float4 result = col;

                if (col.a == 0 && outline > 0)
                {
                    result = _OutlineColor;
                }

                return result;
            }
            ENDCG
        }
    }
}