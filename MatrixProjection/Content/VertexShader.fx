#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix ViewProjection;
matrix World;
float4 Color;
float3 Normal;

float3 LightPos;
float LightPower;
float4 LightColor;
float4 AmbientLightColor;

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float3 Position3D : TEXCOORD0;
    float3 Normal : NORMAL0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    matrix WorldViewProjection = mul(World, ViewProjection);
	output.Position = mul(input.Position, WorldViewProjection);
    output.Normal = normalize(mul(input.Normal, (float3x3)World)); // Remember to cast World to 3x3 to retain only rotation data !!!
    output.Position3D = mul(input.Position, World);
    output.Color = Color;

	return output;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float diffuseLightingFactor = DotProduct(LightPos, input.Position3D, input.Normal);
    diffuseLightingFactor = saturate(diffuseLightingFactor);
    diffuseLightingFactor *= LightPower;
    
    input.Color = Color*(LightColor + diffuseLightingFactor + AmbientLightColor);
    return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};