Shader "WaveVR/Grid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		LOD 100
        Cull Off
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
            fixed4 frag(v2f i) : SV_Target
            {
                float d = (floor(i.vertex.x) > 3 || floor(i.vertex.y % 25) == 24) ? 0 : 1;
                return float4(1, 1, 1, d);
			}
			ENDCG
		}
	}
}
