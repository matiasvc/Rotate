using UnityEngine;
using System.Collections;

public class VignettingEffect : MonoBehaviour
{
	public float intensity = 0.375f;
	
	public Shader vignettShader;
	
	private Material vignetteMaterial;
	
	void Awake()
	{
		vignetteMaterial = new Material(vignettShader);
	}
	
	void OnRenderImage ( RenderTexture source, RenderTexture destination )
	{
		vignetteMaterial.SetFloat ("_Intensity", intensity);
		Graphics.Blit (source, destination, vignetteMaterial, 0);
	}
}
