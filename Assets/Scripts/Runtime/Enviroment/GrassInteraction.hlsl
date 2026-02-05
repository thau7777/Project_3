// GrassInteraction.hlsl

float4 _InteractionPositions[16];
float _InteractionRadii[16];
int _InteractionCount;

// Function 1: For VERTEX shader (displacement only)
void CalculateGrassDisplacement_float(
    float3 WorldPos,
    out float Displacement,
    out float2 Direction
)
{
    Displacement = 0;
    Direction = float2(0, 0);
    
    for (int i = 0; i < _InteractionCount; i++)
    {
        float3 interactorPos = _InteractionPositions[i].xyz;
        float radius = _InteractionRadii[i];
        
        float2 offset = WorldPos.xz - interactorPos.xz;
        float distance = length(offset);
        
        // Normal bending - no clipping here
        float influence = 1.0 - saturate(distance / radius);
        
        if (influence > 0)
        {
            influence = smoothstep(0.0, 1.0, influence);
            Displacement += influence;
            Direction += normalize(offset) * influence;
        }
    }
    
    Displacement = saturate(Displacement);
    
    if (Displacement > 0)
    {
        Direction = normalize(Direction);
    }
}
// Function 2: For FRAGMENT shader (clipping/dithering)
void CalculateGrassCulling_float(
    float3 WorldPos,
    float CullRadiusMutiplier,
    float2 DitherRadiusMinMaxMutiplier,
    out float ShouldClip
)
{
    ShouldClip = 0; // 0 = visible, 1 = clipped
    
    for (int i = 0; i < _InteractionCount; i++)
    {
        float3 interactorPos = _InteractionPositions[i].xyz;
        float radius = _InteractionRadii[i];
        
        float2 offset = WorldPos.xz - interactorPos.xz;
        float distance = length(offset);
        
        // Zone 1: Complete removal (0-20% of radius)
        float cullRadius = radius * CullRadiusMutiplier;
        if (distance < cullRadius)
        {
            clip(-1);
            return;
        }
        
        // Zone 2: Dithered fade (20-45% of radius)
        float ditherStartRadius = radius * DitherRadiusMinMaxMutiplier.x;
        float ditherEndRadius = radius * DitherRadiusMinMaxMutiplier.y;
        
        if (distance < ditherEndRadius)
        {
            float ditherAmount = 1.0 - saturate((distance - ditherStartRadius) / (ditherEndRadius - ditherStartRadius));
            
            // 4x4 Bayer dither matrix
            float4x4 bayerMatrix =
            {
                0.0 / 16.0, 8.0 / 16.0, 2.0 / 16.0, 10.0 / 16.0,
                12.0 / 16.0, 4.0 / 16.0, 14.0 / 16.0, 6.0 / 16.0,
                3.0 / 16.0, 11.0 / 16.0, 1.0 / 16.0, 9.0 / 16.0,
                15.0 / 16.0, 7.0 / 16.0, 13.0 / 16.0, 5.0 / 16.0
            };
            
            float2 matrixPos = fmod(WorldPos.xz * 4.0, 4.0);
            int x = int(matrixPos.x);
            int y = int(matrixPos.y);
            float threshold = bayerMatrix[y][x];
            
            clip(threshold - ditherAmount);
            return;
        }
    }
}