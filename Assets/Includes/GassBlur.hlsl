#ifndef GASSBLUR_INCLUDED
#define GASSBLUR_INCLUDED

void GassBlur_float(float2 UV, float BlurRadius, Texture2D MainTex, SamplerState Sampler, float TextureSize, float DepthValue, out float4 BlurColor)
{
    float4 col = float4(0, 0, 0, 0);
    
    float sigma = BlurRadius / 3.0f;
    float sigma2 = sigma * sigma;
    float left = 1 / (2 * sigma2 * 3.1415926f);

    for (int x = -BlurRadius; x <= BlurRadius; ++x)
    {
        for (int y = -BlurRadius; y <= BlurRadius; ++y)
        {
            float4 color = MainTex.SampleLevel(Sampler, UV + float2(x / TextureSize, y / TextureSize) * DepthValue, 0);
            float weight = left * exp(-(x * x + y * y) / (2 * sigma2));
            col += color * weight;
        }
    }

    BlurColor = col;
}

#endif