using UnityEngine;
using System.Collections;

public static class MeshEx {
	
	public static Mesh TransformMesh(Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		Vector3[] vertsArray = mesh.vertices;
		Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scale);
		
		for (int i = 0; i < vertsArray.Length; i++)
			vertsArray[i] = matrix.MultiplyPoint(vertsArray[i]);
		
		mesh.vertices = vertsArray;
		
		return mesh;
	}
	
	public static Mesh CreateQuad()
	{
		Mesh mesh = new Mesh();
		
		Vector3[] vertsArray = new Vector3[]{
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 1f)
		};
		
		int[] trisArray = new int[]{
			2,1,0,
			2,3,1
		};
		
		Vector2[] uvArray = new Vector2[]{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};

		mesh.name = "quad";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateTwoSidedQuad()
	{
		Mesh mesh = new Mesh();
		
		Vector3[] vertsArray = new Vector3[]{
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 1f)
		};
		
		int[] trisArray = new int[]{
			2,1,0,
			2,0,3,
			2,3,1,
			3,0,1
		};
		
		Vector2[] uvArray = new Vector2[]{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};

		mesh.name = "quad";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateDisk(int subdivisons)
	{
		Mesh mesh = new Mesh();
		
		if (subdivisons < 3)
		{
			Debug.LogError("ERROR! Minimum subdivisons for disk is 3");
			return mesh;
		}
		
		Vector3[] vertsArray 	= new Vector3	[subdivisons+1];
		int[] trisArray 		= new int		[subdivisons*3];
		Vector2[] uvArray 		= new Vector2	[subdivisons+1];
		
		vertsArray[0] = new Vector3(0.5f, 0f, 0.5f); // Center point
		uvArray[0] = new Vector2(0.5f, 0.5f);
		
		float slice = 2f * Mathf.PI / subdivisons;
		
		for (int i = 0; i < subdivisons; i++)
		{
			float angle = slice * i;
			float xPos = Mathf.Cos(angle) * 0.5f + 0.5f;
			float yPos = Mathf.Sin(angle) * 0.5f + 0.5f;
			
			vertsArray[i+1] 	= new Vector3(xPos, 0f, yPos);
			uvArray[i+1] 		= new Vector2(xPos, yPos);
			
			if(i < subdivisons -1)
				trisArray[i*3] 	= i+2;
			else
				trisArray[i*3] 	= 1;
			
			trisArray[i*3+1] 	= i+1;
			trisArray[i*3+2] 	= 0;
		}

		mesh.name = "disk";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateCylinder(int subdivisons)
	{
		Mesh mesh = new Mesh();
		
		if (subdivisons < 3)
		{
			Debug.LogError("ERROR! Minimum subdivisons for cylinder is 3");
			return mesh;
		}
		
		Vector3[] vertsArray 	= new Vector3	[subdivisons*2+2];
		int[] trisArray 		= new int		[subdivisons*6];
		Vector2[] uvArray 		= new Vector2	[subdivisons*2+2];
		
		float slice = 2f * Mathf.PI / subdivisons;
		
		for (int i = 0; i <= subdivisons; i++)
		{
			float angle = slice * i;
			float xPos = Mathf.Cos(angle) * 0.5f + 0.5f;
			float yPos = Mathf.Sin(angle) * 0.5f + 0.5f;
			
			vertsArray[i*2] 	= new Vector3(xPos, 1f, yPos);
			vertsArray[i*2+1] 	= new Vector3(xPos, 0f, yPos);
		}
		
		for (int i = 0; i < subdivisons; i++)
		{
			trisArray[i*6]		= 2*i;
			trisArray[i*6+1]	= 2*i+2;
			trisArray[i*6+2]	= 2*i+3;
			
			trisArray[i*6+3]	= 2*i;
			trisArray[i*6+4]	= 2*i+3;
			trisArray[i*6+5]	= 2*i+1;
		}
		
		float uvSlice = 1f / subdivisons;
		
		for (int i = 0; i <= subdivisons; i++)
		{
			uvArray[i*2] 	= new Vector2(uvSlice*i, 1f);
			uvArray[i*2+1] 	= new Vector2(uvSlice*i, 0f);
		}

		mesh.name = "cylinder";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateTwoSidedCylinder(int subdivisons)
	{
		Mesh mesh = new Mesh();
		
		if (subdivisons < 3)
		{
			Debug.LogError("ERROR! Minimum subdivisons for cylinder is 3");
			return mesh;
		}
		
		Vector3[] vertsArray 	= new Vector3	[subdivisons*2+2];
		int[] trisArray 		= new int		[subdivisons*12];
		Vector2[] uvArray 		= new Vector2	[subdivisons*2+2];
		
		float slice = 2f * Mathf.PI / subdivisons;
		
		for (int i = 0; i <= subdivisons; i++)
		{
			float angle = slice * i;
			float xPos = Mathf.Cos(angle) * 0.5f + 0.5f;
			float yPos = Mathf.Sin(angle) * 0.5f + 0.5f;
			
			vertsArray[i*2] 	= new Vector3(xPos, 1f, yPos);
			vertsArray[i*2+1] 	= new Vector3(xPos, 0f, yPos);
		}
		
		for (int i = 0; i < subdivisons; i++)
		{
			// Outside polygon
			trisArray[i*12]		= 2*i;
			trisArray[i*12+1]	= 2*i+2;
			trisArray[i*12+2]	= 2*i+3;
			
			trisArray[i*12+3]	= 2*i;
			trisArray[i*12+4]	= 2*i+3;
			trisArray[i*12+5]	= 2*i+1;
			
			// Inside polygon
			trisArray[i*12+6]	= 2*i+3;
			trisArray[i*12+7]	= 2*i+2;
			trisArray[i*12+8]	= 2*i;
			
			trisArray[i*12+9]	= 2*i+1;
			trisArray[i*12+10]	= 2*i+3;
			trisArray[i*12+11]	= 2*i;
		}
		
		float uvSlice = 1f / subdivisons;
		
		for (int i = 0; i <= subdivisons; i++)
		{
			uvArray[i*2] 	= new Vector2(uvSlice*i, 1f);
			uvArray[i*2+1] 	= new Vector2(uvSlice*i, 0f);
		}

		mesh.name = "cylinder";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateCube()
	{
		Mesh mesh = new Mesh();
		
		Vector3[] vertsArray = new Vector3[]{
			new Vector3(0f, 0f, 1f), //  0
			new Vector3(1f, 0f, 1f), //  1
			new Vector3(0f, 0f, 1f), //  2
			new Vector3(0f, 1f, 1f), //  3
			new Vector3(1f, 1f, 1f), //  4
			new Vector3(1f, 0f, 1f), //  5
			new Vector3(0f, 0f, 0f), //  6
			new Vector3(0f, 1f, 0f), //  7
			new Vector3(1f, 1f, 0f), //  8
			new Vector3(1f, 0f, 0f), //  9
			new Vector3(0f, 0f, 0f), // 10
			new Vector3(1f, 0f, 0f), // 11
			new Vector3(0f, 0f, 1f), // 12 
			new Vector3(1f, 0f, 1f)  // 13
		};
		
		int[] trisArray = new int[]{
			0,1,4,
			0,4,3,
			2,3,7,
			2,7,6,
			3,4,8,
			3,8,7,
			4,5,9,
			4,9,8,
			7,8,11,
			7,11,10,
			10,11,13,
			10,13,12
		};
		
		Vector2[] uvArray = new Vector2[]{
			new Vector2(0.375f,	1f),	//  0
			new Vector2(0.625f,	1f),	//  1
			new Vector2(0.125f,	0.75f),	//  2
			new Vector2(0.375f, 0.75f),	//  3
			new Vector2(0.625f,	0.75f),	//  4
			new Vector2(0.875f,	0.75f),	//  5
			new Vector2(0.125f,	0.5f),	//  6
			new Vector2(0.375f,	0.5f),	//  7
			new Vector2(0.625f,	0.5f),	//  8
			new Vector2(0.875f,	0.5f),	//  9
			new Vector2(0.375f,	0.25f),	// 10
			new Vector2(0.625f,	0.25f),	// 11
			new Vector2(0.375f,	0f),	// 12
			new Vector2(0.625f,	0f)		// 13
		};

		mesh.name = "cube";
		mesh.vertices = vertsArray;
		mesh.triangles = trisArray;
		mesh.uv = uvArray;
		
		mesh.Optimize();
		
		return mesh;
	}
	
	public static Mesh CreateSphere(int subdivisonsAxis, int subdivisonsHeigth)
	{
		Debug.LogError("ERROR! Fucntion not yet implemented");
		
		Mesh mesh = new Mesh();
		
		return mesh;
	}
	
}
