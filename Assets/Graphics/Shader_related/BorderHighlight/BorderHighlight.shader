Shader "Custom/BorderHighlight"
{
    Properties
    {
        _BorderColor ("Border Color", Color) = (1, 1, 1, 0.5)
        _BorderWidthPixels ("Border Width Pixels", int) = 5
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Tags 
        {
            "RenderType" = "Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2D(_MainTex);

            float4 _BorderColor;
            int _BorderWidthPixels;

            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            struct fragInput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fragInput vert (appdata v)
            {
                fragInput o;

                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;

                return o;
            }

            float4 frag(fragInput i) : SV_TARGET
            {
                float4 currentColor = UNITY_SAMPLE_TEX2D(_MainTex, i.uv);

                if(currentColor[3] == 0)
                {
                    [loop]
                    for (int radiusP = 0; radiusP < _BorderWidthPixels; radiusP++)
                    {
                        for(int x = -1; x <= 1; x++)
                        {
                            for(int y = -1; y <= 1; y++)
                            {
                                if(x == 0 && y == 0)
                                {
                                    continue;
                                }
                                float2 sampleCoordsOffset = {radiusP * x, radiusP * y};
                                if(length(sampleCoordsOffset) > _BorderWidthPixels) //for some reason this is not fully a solution
                                {
                                    continue;
                                }


                                sampleCoordsOffset[0] = sampleCoordsOffset[0] * _MainTex_TexelSize.x;
                                sampleCoordsOffset[1] = sampleCoordsOffset[1] * _MainTex_TexelSize.y;
                                float4 sampledColor = UNITY_SAMPLE_TEX2D(_MainTex, i.uv + sampleCoordsOffset);

                                if(sampledColor[3] != 0 && sampledColor[0] != _BorderColor[0]
                                    && sampledColor[1] != _BorderColor[1] && sampledColor[2] != _BorderColor[2]
                                    && sampledColor[3] != _BorderColor[3])
                                {
                                    return _BorderColor;
                                }
                            }
                        }
                    }
                    return currentColor;
                }
                else
                {
                    currentColor[3] = 0; //might be a problem above
                    currentColor[0] = 0.1;


                    return currentColor;
                }
            }

            ENDHLSL
        }
    }
}
