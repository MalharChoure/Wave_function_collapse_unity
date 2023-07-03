Shader "Unlit/PlatformWalls"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

        Pass
        {
            Cull Back
            ZWrite Off
            ZTest LEqual
            Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct MeshData
            {
                float4 vertex : POSITION;   // vertex positon
                float3 normals : NORMAL;
                float2 uv0 : TEXCOORD0;  // uv0 coordinates
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space POSITION
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);      // local space to clip space
                o.normal = v.normals;
                o.uv = v.uv0;
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // sample the texture

                return float4(0.0, 0.0, (1.0 - i.uv.y + _Time.y / 3.0) % 1.0, (1.0 - i.uv.y + _Time.y) % 1.0);
            }
            ENDCG
        }
    }
}

