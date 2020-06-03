// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageBlendEffect"
{
	Properties
	{
		_MainTex ("Base", 2D) = "" {}
		_BlendTex ("Image", 2D) = "" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _BlendTex;
	sampler2D _BumpMap;
	
	float _BlendAmount;
	float _EdgeSharpness;
	float _SeeThroughness;
	float _Distortion;
		
	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR
	{ 
		float4 blendColor = tex2D(_BlendTex, i.uv);

		blendColor.a = blendColor.a + (_BlendAmount * 2 - 1);
		blendColor.a = saturate(blendColor.a * _EdgeSharpness - (_EdgeSharpness - 1) * 0.5);
		
		//Distortion:
		half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv)).rg;
		float4 mainColor = tex2D(_MainTex, i.uv+bump*blendColor.a*_Distortion);
		
		//return float4(i.uv.x+bump.x,i.uv.y+bump.y,0.5,0)*blendColor.a*_Distortion; //Test
		
		float4 overlayColor = blendColor;
		overlayColor.rgb = mainColor.rgb*(blendColor.rgb+0.5)*(blendColor.rgb+0.5); //double overlay
		
		blendColor = lerp(blendColor,overlayColor,_SeeThroughness);

		return lerp(mainColor, blendColor, blendColor.a);
	}

	ENDCG 
	
	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog
			{
				Mode off
			}

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off	
} 