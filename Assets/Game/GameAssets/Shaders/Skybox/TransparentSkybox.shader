Shader "YShaders/Skybox/TransparentSkybox"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DetailTex ("Detail Texture", 2D) = "transparent" {}
        _DetailTint ("Detail Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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

            fixed4 _Color;
            sampler2D _DetailTex;
            float4 _DetailTex_ST;
            fixed4 _DetailTint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _DetailTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainCol = _Color;
                fixed4 detailCol = tex2D(_DetailTex, i.uv);
                detailCol = detailCol * _DetailTint;

                mainCol.rgb = lerp(mainCol.rgb, detailCol.rgb, detailCol.a);
                mainCol.a = 1;

                return mainCol;
            }
            ENDCG
        }
    }
}
