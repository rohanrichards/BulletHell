#ifndef GRAYSCALE_INCLUDED
#define GRAYSCALE_INCLUDED

void Grayscale_float(float4 input, out float3 output)
{
	output = input.xyz;
	if(input.w<1.0){
		output=float3(0, 0, 0);
	}
}

#endif // GRAYSCALE_INCLUDED