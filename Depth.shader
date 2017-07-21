Shader "Hidden/Depth" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always


        CGINCLUDE
        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };
        struct v2f {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        v2f vert (appdata v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }
        
        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _CameraDepthTexture;

            float4 frag(v2f IN) : SV_Target {
                float d = Linear01Depth(tex2D(_CameraDepthTexture, IN.uv).x);
                return d;
            }

            ENDCG
        }
	}
}
