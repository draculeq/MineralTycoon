Shader "Deadbit/Color"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

		// compile shader into multiple variants, with and without shadows
		// (we don't care about any lightmaps yet, so skip these variants)
		#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
		// shadow helper functions and macros
		#include "AutoLight.cginc"

		struct v2f
		{
			float4 vertex : SV_POSITION;
			SHADOW_COORDS(1) // put shadows data into TEXCOORD1
			fixed3 light : COLOR0;
			fixed3 ambient : COLOR1;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			half3 worldNormal = UnityObjectToWorldNormal(v.normal);
			half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
			o.light = nl * _LightColor0.rgb;
			o.ambient = ShadeSH9(half4(worldNormal,1));
			//TRANSFER_SHADOW(o)
			return o;
		}

		fixed4 _Color;

		fixed4 frag(v2f i) : SV_Target
		{
		fixed4 col = _Color;
		fixed shadow = SHADOW_ATTENUATION(i);
		fixed3 lighting = i.light * shadow + i.ambient;
		col.rgb *= lighting;

		return col;
	}
	ENDCG
}
// shadow casting support
//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}