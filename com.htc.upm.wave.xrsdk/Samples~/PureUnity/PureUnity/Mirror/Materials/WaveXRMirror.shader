// original source from: http://wiki.unity3d.com/index.php/MirrorReflection4
Shader "Wave/XRMirror"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		[HideInInspector] _ReflectionTexLeft("_ReflectionTexLeft", 2D) = "white" {}
		[HideInInspector] _ReflectionTexRight("_ReflectionTexRight", 2D) = "white" {}

		_Transparency("Transparency", Range(0.0, 1.0)) = 0.25
	}

		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 refl : TEXCOORD1;
				float2 idx : TEXCOORD2;
				float4 pos : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;
			float4 _TintColor;
			float _Transparency;
			float _CutoutThresh;
			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// For stereo camera, Unity will choose a variation which unity_StereoEyeIndex will always be zero.  Use variable pass to fragment shader.
				o.idx.x = unity_StereoEyeIndex;
				o.idx.y = unity_StereoEyeIndex;
				o.refl = ComputeScreenPos(o.pos);

				return o;
			}

			sampler2D _MainTex;
			sampler2D _ReflectionTexLeft;
			sampler2D _ReflectionTexRight;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 refl;
				if (i.idx.x < 0.5f) refl = tex2Dproj(_ReflectionTexLeft, UNITY_PROJ_COORD(i.refl));
				else  refl = tex2Dproj(_ReflectionTexRight, UNITY_PROJ_COORD(i.refl));

				fixed4 col = tex2D(_MainTex, i.uv);
				return fixed4((col * refl).xyz, _Transparency);
			}
			ENDCG
		}
	}
}
