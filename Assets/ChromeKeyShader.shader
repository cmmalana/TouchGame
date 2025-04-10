Shader "Custom/ChromaKeyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
        _ChromaKey ("Chroma Key Color", Color) = (0,1,0,1)
        _Threshold ("Threshold", Range(0,1)) = 0.3
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _ChromaKey;
            float _Threshold;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Calculate difference from the chroma key color
                float diff = distance(col.rgb, _ChromaKey.rgb);
                
                // If the difference is below threshold, make pixel transparent
                if(diff < _Threshold)
                {
                    col.a = 0;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
