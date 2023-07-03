Shader "Unlit/PlayerPlatform"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend DstColor   Zero

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
                float4 color = float4(52/255.0, 225/255.0, 235.0/255.0, 0.5);
                if (i.uv.x < 0.01 || i.uv.x > 0.99 || i.uv.y < 0.01 || i.uv.y > 0.99){
                    color *= 0.01;
                }
                return color;
            }
            ENDCG
        }
    }
}
