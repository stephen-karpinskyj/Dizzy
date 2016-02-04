Shader "Custom/Starfield" 
{
	Properties
	{
		_Stars ("Stars (RGB)", 2D) = "white" { }	

	}
	SubShader 
	{
		Pass
		{
			Tags { "Queue" = "Opaque"}
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
		
			CGPROGRAM
			#pragma exclude_renderers xbox360 ps3 flash
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			sampler2D _Stars;

			struct vertexInput
			{
      			float4 vertex : POSITION;
      			float4 texcoord : TEXCOORD0;
      			float4 texcoord1 : TEXCOORD1;
      			float4 color : COLOR;
				float4 normal : NORMAL;
			};
      
			struct fragmentInput {
        		float4 uv1 : TEXCOORD0;
        		float2 uv2 : TEXCOORD1;
        		
        		float4 pos : SV_POSITION;
        		float4 color : COLOR;
			};

			fragmentInput vert (vertexInput v) 
			{
				fragmentInput o;
				o.uv1 = v.texcoord.xyyy;
				o.uv1.y -= _Time.x*.15;
				o.uv1.z -= _Time.x*.3;
				o.uv1.w -= _Time.x*.45;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			half4 frag (fragmentInput v):COLOR
			{
				half4 sm_stars = tex2D(_Stars,v.uv1.xy);
				half4 md_stars = tex2D(_Stars,v.uv1.xz);
				half4 lg_stars = tex2D(_Stars,v.uv1.xw);
				
				half3 _sm_stars = lerp(0,0.4,sm_stars.b);
				half3 _md_stars = lerp(0,0.4,md_stars.g);
				half3 _lg_stars = lerp(0,0.4,lg_stars.r);
				
				half4 combined = half4(_sm_stars+_md_stars+_lg_stars,1);

				return combined;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
