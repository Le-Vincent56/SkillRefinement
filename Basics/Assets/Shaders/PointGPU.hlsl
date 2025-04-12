#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3> _Positions;
#endif

float _Step;

void ConfigureProcedural()
{
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			float3 position = _Positions[unity_InstanceID];

			// The transformation matrix is provided globally via unity_ObjectToWorld;
			// because we're drawing procedurally it's an identity matrix, so we have to replace it
			unity_ObjectToWorld = 0.0;
	
			// Construct a column vector for the position offset and set it at the fourth column
			unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
	
			// Correctly scale the points
			unity_ObjectToWorld._m00_m11_m22 = _Step;
#endif
}

void ShaderGraphFunction_float(float3 In, out float3 Out)
{
    Out = In;
}

void ShaderGraphFunction_half(half3 In, out half3 Out)
{
    Out = In;
}