Shader "HighlightAdditive"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_Speed_MainTex("_Speed_MainTex", Float) = 2
_Gradient("_Gradient", 2D) = "black" {}
_Speed_Gradient("_Speed_Gradient", Float) = 10
_Specular("_Specular", Float) = 0
_Gloss("_Gloss", Float) = 0
_Fresnel("_Fresnel", Float) = 0.1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="True"
"RenderType"="Transparent"

		}

		
Cull Back
ZWrite Off
ZTest LEqual
ColorMask RGBA
Blend One One
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _MainTex;
float _Speed_MainTex;
sampler2D _Gradient;
float _Speed_Gradient;
float _Specular;
float _Gloss;
float _Fresnel;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float4 color : COLOR;
float3 viewDir;
float2 uv_MainTex;
float2 uv_Gradient;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Fresnel0_1_NoInput = float4(0,0,1,1);
float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel0_1_NoInput.xyz ) )).xxxx;
float4 Multiply0=_Speed_MainTex.xxxx * _Time;
float4 UV_Pan0=float4((IN.uv_MainTex.xyxy).x,(IN.uv_MainTex.xyxy).y + Multiply0.x,(IN.uv_MainTex.xyxy).z,(IN.uv_MainTex.xyxy).w);
float4 Tex2D0=tex2D(_MainTex,UV_Pan0.xy);
float4 Add0=Fresnel0 + Tex2D0;
float4 Multiply2=IN.color * Add0;
float4 SplatAlpha0=IN.color.w;
float4 Multiply1=_Time * _Speed_Gradient.xxxx;
float4 UV_Pan1=float4((IN.uv_Gradient.xyxy).x,(IN.uv_Gradient.xyxy).y + Multiply1.x,(IN.uv_Gradient.xyxy).z,(IN.uv_Gradient.xyxy).w);
float4 Tex2D1=tex2D(_Gradient,UV_Pan1.xy);
float4 Multiply4=SplatAlpha0 * Tex2D1;
float4 Add1=Multiply2 + Multiply4;
float4 Multiply3=SplatAlpha0 * _Specular.xxxx;
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = Add1;
o.Specular = _Gloss.xxxx;
o.Gloss = Multiply3;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}