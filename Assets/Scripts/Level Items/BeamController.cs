using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeamController : MonoBehaviour {
	
	
	private Material mat;
	private float offset = 0;
	private const float offsetSpeed = 2.5f;
	
	void Awake()
	{
		mat = gameObject.renderer.material;
	}
	
	
	public void GenerateBeamMesh(int heigth)
	{
		if(heigth < 1)
		{
			Debug.LogError("Invalid Heigth;");
			return;
		}
		
		List<Vector3> verticesList = new List<Vector3>();
		List<int> trianglesList = new List<int>();
		List<Vector2> uvList = new List<Vector2>();
		
		verticesList.Add(new Vector3(-0.2f, 0f, 0.3f));
		verticesList.Add(new Vector3(-0.2f, 0f, 0.7f));
		
		trianglesList.Add(0);
		trianglesList.Add(1);
		trianglesList.Add(3);
		
		trianglesList.Add(0);
		trianglesList.Add(3);
		trianglesList.Add(2);
		
		uvList.Add(new Vector2(0.3f, 1.05f));
		uvList.Add(new Vector2(0.7f, 1.05f));
		
		for (int i = 0; i <= heigth; i++)
		{
			verticesList.Add(new Vector3(1f * i, 0f, 0.3f));
			verticesList.Add(new Vector3(1f * i, 0f, 0.7f));
			
			if(i < heigth)
			{
				trianglesList.Add(0 + 2 + (2 * i));
				trianglesList.Add(1 + 2 + (2 * i));
				trianglesList.Add(3 + 2 + (2 * i));
				
				trianglesList.Add(0 + 2 + (2 * i));
				trianglesList.Add(3 + 2 + (2 * i));
				trianglesList.Add(2 + 2 + (2 * i));	
			}
			
			uvList.Add(new Vector2(0.3f, 1f - (1f / 4f) * i));
			uvList.Add(new Vector2(0.7f, 1f - (1f / 4f) * i));
		}
		
		verticesList.Add(new Vector3(1f * heigth + 0.2f, 0f, 0.3f));
		verticesList.Add(new Vector3(1f * heigth + 0.2f, 0f, 0.7f));
		
		trianglesList.Add(0 + 2 + (2 * heigth));
		trianglesList.Add(1 + 2 + (2 * heigth));
		trianglesList.Add(3 + 2 + (2 * heigth));
		
		trianglesList.Add(0 + 2 + (2 * heigth));
		trianglesList.Add(3 + 2 + (2 * heigth));
		trianglesList.Add(2 + 2 + (2 * heigth));
		
		uvList.Add(new Vector2(0.3f, (1f - (1f / 4f) * heigth) - 0.05f));
		uvList.Add(new Vector2(0.7f, (1f - (1f / 4f) * heigth) - 0.05f));
		
		Mesh mesh = new Mesh();
		mesh.name = "beamMesh";
		
		mesh.vertices = verticesList.ToArray();
		mesh.triangles = trianglesList.ToArray();
		mesh.uv = uvList.ToArray();
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		meshFilter.sharedMesh = mesh;
	}
	
	void Update()
	{
		if(offset > 1f)
		{
			offset -= 1f;
		}
		
		mat.mainTextureOffset = new Vector2(0f, offset);
		
		offset += offsetSpeed * Time.deltaTime;
	}
}
