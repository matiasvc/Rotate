Shader "Color"
{
	Properties 
	{
_Texture("_Texture", 2D) = "black" {}
_Gradient("_Gradient", 2D) = "black" {}
_Normal("_Normal", 2D) = "black" {}
_GradientPower("_GradientPower", Float) = 0

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _Texture;
sampler2D _Gradient;
sampler2D _Normal;
float _GradientPower;

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
				float2 uv_Normal;
float2 uv_Texture;
float3 viewDir;

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
				
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_Normal,(IN.uv_Normal.xyxy).xy)).xyz, 1.0 );
float4 Tex2D0=tex2D(_Texture,(IN.uv_Texture.xyxy).xy);
float4 Fresnel1_1_NoInput = float4(0,0,1,1);
float4 Fresnel1=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel1_1_NoInput.xyz ) )).xxxx;
float4 Pow0=pow(Fresnel1,_GradientPower.xxxx);
float4 Tex2D1=tex2D(_Gradient,Pow0.xy);
float4 Multiply0=Tex2D0 * Tex2D1;
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Normal = Tex2DNormal0;
o.Emission = Multiply0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}