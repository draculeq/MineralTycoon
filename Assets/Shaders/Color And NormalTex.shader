Shader "Deadbit/Color And Normal Texture" 
{
	Properties
	{

		_MainTex("Texture", 2D) = "bump" {}
		_Color("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		sampler2D _MainTex;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 col = _Color;
			o.Albedo = col;
			o.Normal = UnpackNormal(tex2D(_MainTex, IN.uv_MainTex));
		}
		ENDCG
	}
	Fallback "Diffuse"
}