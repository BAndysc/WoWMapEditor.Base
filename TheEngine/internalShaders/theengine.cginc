#if VERTEX_SHADER
cbuffer SceneBuffer : register(b12)
{
    matrix viewMatrix;
    matrix projectionMatrix;
};

cbuffer ObjectBuffer : register(b13)
{
    matrix worldMatrix;
};
#endif

#if PIXEL_SHADER
cbuffer SceneBuffer : register(b13)
{
    float4 lightPosition;
    float4 lightColor;
};
#endif