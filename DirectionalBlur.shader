Shader "Hidden/DirectionalBlur" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
        _AmountTex ("Amount", 2D) = "white" {}
        _Dir ("Direction", Range(0, 1)) = 0
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

        CGINCLUDE
        #include "UnityCG.cginc"
               
        #define GAUSS_N 256
        static const float TWO_PI = 6.283185;

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };
        struct v2f {
            float2 uv : TEXCOORD0;
            float2 dp : TEXCOORD1;
            float4 vertex : SV_POSITION;
        };

        float _Dir;

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        sampler2D _AmountTex;

        v2f vert (appdata v) {
            static float2 dp = - float2(cos(_Dir * TWO_PI), sin(_Dir * TWO_PI)) * _MainTex_TexelSize.xy;

            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.dp = dp;
            return o;
        }
        ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 frag (v2f IN) : SV_Target {
                float4 csrc = tex2D(_MainTex, IN.uv);
                float t = saturate(tex2D(_AmountTex, IN.uv).x);

                float4 csum = float4(csrc.xyz, 1);
                float invSigma2exp = 1.0 / (0.5 * t * t * GAUSS_N * GAUSS_N);
                for (int i = 1; i < GAUSS_N; i++) {
                    float w = exp(- i * i * invSigma2exp);
                    float4 c = tex2D(_MainTex, frac(IN.uv + i * IN.dp));
                    csum += w * float4(c.xyz, 1);
                }
				csum /= csum.w;

                return csum;
			}
			ENDCG
		}
	}
}
