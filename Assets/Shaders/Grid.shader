﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Deadbit/Grid" {
	Properties
	{
		_GridThickness("Grid Thickness", Float) = 0.01
		_GridSpacingX("Grid Spacing X", Float) = 1.0
		_GridSpacingY("Grid Spacing Y", Float) = 1.0
		_GridOffsetX("Grid Offset X", Float) = .5
		_GridOffsetY("Grid Offset Y", Float) = .5
		_GridColour("Grid Colour", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform float _GridThickness;
			uniform float _GridSpacingX;
			uniform float _GridSpacingY;
			uniform float _GridOffsetX;
			uniform float _GridOffsetY;
			uniform float4 _GridColour;
			uniform float4 _BaseColour;
	
			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput 
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
			};

			// VERTEX SHADER
			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.worldPos = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}

			// FRAGMENT SHADER
			float4 frag(vertexOutput input) : COLOR
			{
				if (frac((input.worldPos.x + _GridOffsetX) / _GridSpacingX) < (_GridThickness / _GridSpacingX) ||
				frac((input.worldPos.z + _GridOffsetY) / _GridSpacingY) < (_GridThickness / _GridSpacingY)) 
				{
					return _GridColour;
				}
				else 
				{
					return _BaseColour;
				}
			}
		ENDCG
		}
	}
}