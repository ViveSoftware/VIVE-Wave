// Lucifer
// https://zhuanlan.zhihu.com/p/159913409
// https://zhuanlan.zhihu.com/p/163095303

// Quaker: This shader is not ok for Android

Shader"Unlit/UnlitLiquidShader"
{
    Properties
    {
        _LiquidColor ("LiquidColor", Color) = (0.2,0.1,0.1,0.9)
        _BBoxPX ("Bounding Box +X", Vector) = ( 1, 0, 0, 1)
        _BBoxNX ("Bounding Box -X", Vector) = (-1, 0, 0, 1)
        _BBoxPY ("Bounding Box +Y", Vector) = ( 0, 1, 0, 1)
        _BBoxNY ("Bounding Box -Y", Vector) = ( 0,-1, 0, 1)
        _BBoxPZ ("Bounding Box +Z", Vector) = ( 0, 0, 1, 1)
        _BBoxNZ ("Bounding Box -Z", Vector) = ( 0, 0,-1, 1)
        _GravityUp ("Up normal", Vector) = ( 0, 1, 0, 0)
        _WaterLevel ("WaterLevel", range(0.05, 1.2)) = 1.0
        _Noise ("Noise Power", Range(0, 1)) = 0
        _RimPower ("Rim Power", Range(0, 10)) = 0.0
        _MeshScale ("Mesh Scale", Range(0, 1)) = 0.98
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100

        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha
            Cull off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
//            #pragma hardware_tier_variants

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float4 worldPos : TEXCOORD0;
                fixed2 hl : TEXCOORD1;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _WaterLevel;
            float4 _LiquidColor;
            float _Rim, _RimPower;
            fixed4 _BBoxPX;
            fixed4 _BBoxNX;
            fixed4 _BBoxPY;
            fixed4 _BBoxNY;
            fixed4 _BBoxPZ;
            fixed4 _BBoxNZ;
            fixed4 _GravityUp;
            float _Noise;
            float _MeshScale;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                float4 localPos = float4(v.vertex.xyz * _MeshScale, 1);

                fixed v0 = dot(mul(unity_ObjectToWorld, _BBoxPX), _GravityUp);
                fixed v1 = dot(mul(unity_ObjectToWorld, _BBoxNX), _GravityUp);
                fixed v2 = dot(mul(unity_ObjectToWorld, _BBoxPY), _GravityUp);
                fixed v3 = dot(mul(unity_ObjectToWorld, _BBoxNY), _GravityUp);
                fixed v4 = dot(mul(unity_ObjectToWorld, _BBoxPZ), _GravityUp);
                fixed v5 = dot(mul(unity_ObjectToWorld, _BBoxNZ), _GravityUp);
                o.hl.x = max(max(max(v0, v1), max(v2, v3)), max(v4, v5));
                o.hl.y = min(min(min(v0, v1), min(v2, v3)), min(v4, v5));
                //o.hl.x = max(max(max(v0, v1), max(v2, v3)), max(v4, v5));
                //o.hl.y = min(min(min(v0, v1), min(v2, v3)), min(v4, v5));
                o.vertex = UnityObjectToClipPos(localPos);
                o.worldPos = mul(unity_ObjectToWorld, localPos);
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                return o;
            }

            fixed2 randVec(fixed2 value)
            {
                fixed2 vec = fixed2(dot(value, fixed2(127.1, 337.1)), dot(value, fixed2(269.5, 183.3)));
                vec = -1 + 2 * frac(sin(vec) * 43758.5453123);
                return vec;
            }

            float perlinNoise(float2 uv)
            {
                float a, b, c, d;
                float x0 = floor(uv.x);
                float x1 = ceil(uv.x);
                float y0 = floor(uv.y);
                float y1 = ceil(uv.y);
                fixed2 pos = frac(uv);
                a = dot(randVec(fixed2(x0, y0)), pos - fixed2(0, 0));
                b = dot(randVec(fixed2(x0, y1)), pos - fixed2(0, 1));
                c = dot(randVec(fixed2(x1, y1)), pos - fixed2(1, 1));
                d = dot(randVec(fixed2(x1, y0)), pos - fixed2(1, 0));
                float2 st = 6 * pow(pos, 5) - 15 * pow(pos, 4) + 10 * pow(pos, 3);
                a = lerp(a, d, st.x);
                b = lerp(b, c, st.x);
                a = lerp(a, b, st.y);
                return a;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // rim light
                float dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
                float4 RimResult = smoothstep(0.5, 1.0, dotProduct);
                RimResult.w = 0;

                fixed height = i.hl.x - i.hl.y;

                float waterLevel = i.hl.y + _WaterLevel * height;

                float noiseValue = 0.5 * abs(frac(i.worldPos.xz + i.worldPos.zx + float2(_Time.y, 1.5 * _Time.y)) - 0.5);
                waterLevel += 0.25 * _Noise * perlinNoise(noiseValue) * height;

                clip(waterLevel - i.worldPos.y);
                return _LiquidColor + RimResult;
            }
            ENDCG
        }
    }
}
