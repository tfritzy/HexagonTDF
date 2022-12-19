Shader "Voxel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        [MaterialToggle] _TextureOn ("Texture On", float) = 1
        [MaterialToggle] _LightingOn ("Lighting On", float) = 1
        [MaterialToggle] _AOOn ("AO On", float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
        LOD 100
        Lighting Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vertFunction
            #pragma fragment fragFunction
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                // These are 0 or 1 for each side of the block
                // x is side1 y is side2 z is corner
                float3 sides : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;

            float _TextureOn;
            float _LightingOn;
            float _AOOn;
            fixed4 _Color;
            
            int vertexAO(bool side1, bool side2)
            {
                if(side1 && side2)
                {
                    return 0;
                }
                return (2 - (side1+side2));
            }

            float AOToOcclusion(int ao)
            {
                if(ao == 0)
                {
                    return 0.85;
                }
                if(ao == 1)
                {
                    return .95;
                }
                if(ao == 2)
                {
                    return 1;
                }
                return 0;
            }

            v2f vertFunction (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                if(_LightingOn)
                {
                    float4 lightColor = _Color * dot(v.normal, _WorldSpaceLightPos0);
                    v.color *= lightColor;
                } 
                else {
                    v.color = _Color;
                }

                if(_AOOn)
                {
                    v.color *= AOToOcclusion(vertexAO(v.sides.x, v.sides.y));
                }
                
                o.color = v.color;

                return o;
            }

            fixed4 fragFunction (v2f i) : SV_Target
            {
                fixed4 col;
                if(_TextureOn)
                {
                    col = tex2D(_MainTex, i.uv);
                } else
                {
                    col = fixed4(1,1,1,1);
                }
                
                clip(col.a -1);

                col *= i.color;
                
                return col;
            }

            ENDCG
        }
    }
}
