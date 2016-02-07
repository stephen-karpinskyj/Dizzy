Shader "Custom/Starfield" 
{
	Properties
	{
		_Stars ("Stars (RGB)", 2D) = "white" { }
		_Nebula ("Nebula (RGB)",2D) = "white" {}
		_Blast ("Blast Properties",Vector) = (0,0,1,.1)		//X = x position,Y = y position,Z = radius,W = height
		_BlastColor ("Blast Color",Vector) = (3,1,3,1)		

	}
	SubShader 
	{
		Pass
		{
			Tags { "Queue" = "Opaque"}
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
		
			CGPROGRAM
            //#pragma target 3.0
			#pragma glsl
			#pragma exclude_renderers xbox360 ps3 flash
			//#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
					
            uniform float4 _Offset12;
            uniform float4 _Offset34;
                    
			uniform int _Points_Length = 0;
			uniform float4 _Points [99];		// x-pos, y-pos, radius, intensity
			uniform float3 _Colour [99]; 		// rgb

			sampler2D _Stars;
			sampler2D _Nebula;

			float4 _Blast;
			half4 _BlastColor;

			struct vertexInput
			{
      			float4 vertex : POSITION;
      			float4 texcoord : TEXCOORD0;
      			float4 color : COLOR;
				float4 normal : NORMAL;
			};
      
			struct fragmentInput {
        		float4 uv1 : TEXCOORD0;
        		float4 uv2 : TEXCOORD1;
        		float4 uv3 : TEXCOORD2;
        		float4 blastColor : TEXCOORD3;
        		float4 pos : SV_POSITION;
        		float4 color : COLOR;
			};

			fragmentInput vert (vertexInput v) 
			{
				fragmentInput o;
                
                // Star uv scrolling
				o.uv1 = v.texcoord.xyxy;
                o.uv3 = v.texcoord.xyww;
                
                o.uv1.xy += _Offset12.xy;
                o.uv1.zw += _Offset12.zw;
                o.uv3.xy += _Offset34.xy;
                
                // Nebula uv scrolling
				o.uv2 = v.texcoord.xyxy;

				o.uv2.x += _Time.x*.1;
				o.uv2.y -= _Time.x*.1;
				o.uv2.z -= _Time.x*.1;

				half3 worldPos = mul(_Object2World,v.vertex).xyz;
				
				// Loops over all the points
				half4 h = 0;
				for (int i = 0; i < _Points_Length; i ++)
				{
					// Calculates the contribution of each point
					half di = distance(worldPos.xy, _Points[i].xy);
 
					half ri = _Points[i].z;
					half hi = 1 - saturate(di / ri);
 
					h += hi * _Points[i].w* half4(_Colour[i],1);
				}
				o.color = h;

				//handle blast				
				float4 origin = float4(_Blast.x,_Blast.y,20,1);
         		half3 blastNormal = (normalize(worldPos.xyz-origin.xyz));
         		float dis = max(1-abs(_Blast.z-distance(worldPos.xyz,origin.xyz)*.5),0);
          		o.blastColor = float4(dis.xxx,0)*_Blast.w;  
          		v.vertex.xyz += dis*_Blast.w*float3(blastNormal.x,1,blastNormal.y);
          		//end of blast section

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			half4 frag (fragmentInput v):COLOR
			{
				half4 sm_stars = tex2D(_Stars,v.uv1.xy);
				half4 md_stars = tex2D(_Stars,v.uv1.zw);
				half4 lg_stars = tex2D(_Stars,v.uv3.xy);

				half4 a_nebula = tex2D(_Nebula,v.uv2.xy);
				half4 b_nebula = tex2D(_Nebula,v.uv2.zw);
				
				half3 _sm_stars = lerp(0,0.5,sm_stars.b);
				half3 _md_stars = lerp(0,0.5,md_stars.g);
				half3 _lg_stars = lerp(0,0.5,lg_stars.r);
				
				half4 stars_combined = half4(_sm_stars+_md_stars+_lg_stars,1)+v.color*a_nebula.r*b_nebula.g*3+(v.blastColor*_BlastColor);

				return stars_combined;				
                //return float4(v.color.rgb,1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
