#if VERTEX_SHADER
cbuffer SceneBuffer : register(b12)
{
    matrix viewMatrix;
    matrix projectionMatrix;
    float time;

    float align1;
    float align2;
    float align3;
};

cbuffer ObjectBuffer : register(b13)
{
    matrix worldMatrix;
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
#endif