Shader "Deadbit/Gradient"
{
	Properties
	{
		_Color2("Top Color", Color) = (1,1,1,1)
		_Color("Bottom Color", Color) = (1,1,1,1)
		_Offset("Offset", Float) = 1
	}
	SubShader
	{
		Pass
		{
			// indicate that our pass is the "base" pass in forward
			// rendering pipeline. It gets ambient and main directional
			// light data set up; light direction in _WorldSpaceLightPos0
			// and color in _LightColor0
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "Lighting.cginc"
//#include "UnityLightingCommon.cginc" // for _LightColor0
	
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "AutoLight.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				SHADOW_COORDS(1) // put shadows data into TEXCOORD1
				fixed4 diff : COLOR0; // diffuse lighting color
				fixed4 light : COLOR2; // diffuse lighting color
				fixed3 ambient : COLOR1;
			};			

			fixed4 _Color;
			fixed4 _Color2;
			half _Offset;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = lerp(_Color, _Color2, worldNormal.y+_Offset);
				o.light = nl * _LightColor0;
				o.ambient = ShadeSH9(half4(worldNormal, 1));
				TRANSFER_SHADOW(o)
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = i.diff;
				fixed shadow = SHADOW_ATTENUATION(i);
				fixed3 lighting = i.light * shadow + i.ambient;
				col.rgb *= lighting;
				return col;
			}
			ENDCG
		}
	}
}