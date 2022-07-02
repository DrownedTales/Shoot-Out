// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Hologram"
{
    Properties
    {
		_Color("Color", Color) = (0, 1, 1, 1)
		_RimColor("RimColor", Color) = (0, 1, 1, 1)
		_MainTex("Base Texture", 2D) = "white" {}
		_AlphaTexture1 ("Alpha Mask 1", 2D) = "white" {}
		_AlphaTexture2 ("Alpha Mask 2", 2D) = "white" {}
		//Alpha Mask Properties
		_Scale1 ("Alpha 1 Tiling", Float) = 3
		_Scale2 ("Alpha 2 Tiling", Float) = 3
		_ScrollSpeed1("Alpha 1 scroll Speed", Range(-5, 5)) = 1.0
		_ScrollSpeed2("Alpha 2 scroll Speed", Range(-5, 5)) = 1.0
		// Glow
		_GlowIntensity ("Glow Intensity", Range(0.01, 1.5)) = 0.5
		// Glitch
		_GlitchSpeed ("Glitch Speed", Range(0, 30)) = 10.0
		_GlitchIntensity ("Glitch Intensity", Range(0.0, 0.3)) = 0
		_GlitchAmount ("Glitch Amount", Range(0.0, 30)) = 7
        _GlitchTime ("Time Between Glitches", Range(0.0, 10.0)) = 3
        _DoGlitch("Activate Glitch (scripted)", Int) = 0
        _CurrentTime("Current Time (scripted)", Float) = 0
    }
    SubShader
    {
		Tags{ "Queue" = "Transparent" 
		"RenderType" = "Transparent" 
		"RenderPipeline"="UniversalPipeline" 
		"UniversalMaterialType" = "Unlit" }

		Pass
		{
            //Transparency without surface
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

			CGPROGRAM
				
				#pragma vertex vertexFunc
				#pragma fragment fragmentFunc

				#include "UnityCG.cginc"

				struct appdata{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f{
					float4 position : POSITION;
					float2 uv : TEXCOORD0;
					float3 viewDir : TEXCOORD1;
					float3 alphaPos1 : TEXCOORD2;
					float3 alphaPos2 : TEXCOORD3;
					float3 worldNormal : NORMAL;
				};

				fixed4 _Color, _RimColor;
				sampler2D _MainTex, _AlphaTexture1, _AlphaTexture2;
				half _Scale1, _Scale2, _ScrollSpeed1, _ScrollSpeed2, _GlowIntensity, 
                    _GlitchSpeed, _GlitchIntensity, _GlitchTime, _GlitchAmount, _CurrentTime;
                int _DoGlitch;

				v2f vertexFunc(appdata IN){
					v2f OUT;

					//Glitch
					IN.vertex.x += sin((_Time.y - _CurrentTime) * _GlitchSpeed * _GlitchAmount * IN.vertex.y) * _GlitchIntensity * _DoGlitch;
					IN.vertex.z += sin((_Time.y - _CurrentTime) * _GlitchSpeed * _GlitchAmount * IN.vertex.y + 10) * _GlitchIntensity * _DoGlitch;
					IN.vertex.y += sin((_Time.y - _CurrentTime) * _GlitchSpeed * _GlitchAmount * 0.25 * IN.vertex.y + 20) * _GlitchIntensity * _DoGlitch;

					OUT.position = UnityObjectToClipPos (IN.vertex);
					OUT.uv = IN.uv;

					//Alpha mask
					//OUT.alphaPos1 = ComputeScreenPos(OUT.position) / ObjSpaceViewDir(IN.vertex);
					//OUT.alphaPos2 = ComputeScreenPos(OUT.position) / ObjSpaceViewDir(IN.vertex);
					float4 a = ComputeScreenPos(OUT.position);
					float4 b = ComputeScreenPos(OUT.position);
					OUT.alphaPos1 = a.xyz / a.w;
					OUT.alphaPos2 = b.xyz / b.w;

					//Scroll Alpha mask uv
					OUT.alphaPos1.y += _Time.y * _ScrollSpeed1 / 3;
					OUT.alphaPos2.y += _Time.y * _ScrollSpeed2 / 3;

					OUT.worldNormal = normalize(UnityObjectToWorldNormal(IN.normal));
					OUT.viewDir = normalize(ObjSpaceViewDir(IN.vertex));

					return OUT;
				}

				fixed4 fragmentFunc(v2f IN) : SV_Target{
                    // Alpha Mask
					fixed4 alphaColor = tex2D(_AlphaTexture1,  IN.alphaPos1.xy * _Scale1) + 
                        tex2D(_AlphaTexture2,  IN.alphaPos2.xy * _Scale2);

					fixed4 pixelColor = tex2D(_MainTex, IN.uv);

					pixelColor.w = alphaColor.x;

					// Rim
					half rim = (1.0-abs(dot(IN.viewDir, IN.worldNormal))) * pow(_GlowIntensity, 2);

					return (pixelColor * _Color) * (1-rim) + (rim * _RimColor);
				}
			ENDCG
		}
    }
}
