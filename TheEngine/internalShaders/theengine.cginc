#if VERTEX_SHADER
cbuffer SceneBuffer : register(b12)
{
    row_major matrix viewMatrix;
    row_major matrix projectionMatrix;
    float time;

    float align1;
    float align2;
    float align3;
};

cbuffer ObjectBuffer : register(b13)
{
    row_major matrix worldMatrix;
};
#endif

#if PIXEL_SHADER
cbuffer SceneBuffer : register(b13)
{
    float4 lightDirection;
    float4 lightColor;

    float time;

    float align1;
    float align2;
    float align3;
};

SamplerState TheDefaultSampler : register(s15);

#define tex3D(TEX, UV, INDEX) (TEX.Sample(TheDefaultSampler, float3(UV, INDEX)))
#define tex2D(TEX, UV) (TEX.Sample(TheDefaultSampler, UV))
#endif