using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;
using Poly2Tri;

public static class MeshGenerator {

	private const float CLIPPER_SCALE = 1000f;

	public static Mesh GenerateShapeMesh( Vector2 tileSize, Shape2D shapeOutline ) {

		List<CombineInstance> meshInstances = new List<CombineInstance>();

		Rect meshBounds = shapeOutline.GetBounds();

		int columns = Mathf.CeilToInt( meshBounds.width / tileSize.x ); // We ceil these two guys so the amount of tiles will be just enough to cover the entire shape
		int rows = Mathf.CeilToInt( meshBounds.height / tileSize.y );

		for ( int iCol=0; iCol<columns; iCol++ ) {
			for ( int iRow=0; iRow<rows; iRow++ ) {
				Vector2 topLeft =     new Vector3(meshBounds.x + iCol*tileSize.x,      meshBounds.y + (iRow+1)*tileSize.y);
				Vector2 topRight =    new Vector3(meshBounds.x + (iCol+1)*tileSize.x,  meshBounds.y + (iRow+1)*tileSize.y);
				Vector2 bottomLeft =  new Vector3(meshBounds.x + iCol*tileSize.x,      meshBounds.y + iRow*tileSize.y);
				Vector2 bottomRight = new Vector3(meshBounds.x + (iCol+1)*tileSize.x,  meshBounds.y + iRow*tileSize.y);
				
				Shape2D tileShape = new Shape2D();
				tileShape.AddRange( new Vector2[]{ topLeft, topRight, bottomRight, bottomLeft } );
				
				Mesh mesh = new Mesh();

				if ( shapeOutline.Contains( tileShape ) ) { // The tile is square, so no need for clipping
					mesh = GenerateQuad( topLeft, topRight, bottomLeft, bottomRight );
				} else { // The tile is somehow clipped, so we need to involve the Clipper and Poly2Tri libs
					mesh = GenerateClippedQuad( tileShape, shapeOutline, ClipType.ctIntersection );
				}
					
				CombineInstance meshInstance = new CombineInstance();
				meshInstance.mesh = mesh;
				meshInstance.subMeshIndex = 0;
				meshInstance.transform = Matrix4x4.identity;
				
				meshInstances.Add( meshInstance );
			}
		}
	

		Mesh shapeMesh = new Mesh();
		shapeMesh.name = "shapeMesh";
		shapeMesh.CombineMeshes( meshInstances.ToArray(), true );
		shapeMesh.Optimize();
		shapeMesh.RecalculateNormals();

		return shapeMesh;
	}

	public static Mesh GeneratePlaneMesh( Vector2 tileSize, Rect planeOutline, Shape2D shapeOutline ) {

		List<CombineInstance> meshInstances = new List<CombineInstance>();
		
		int columns = Mathf.CeilToInt( planeOutline.width / tileSize.x ); // We ceil these two guys so the amount of tiles will be just enough to cover the entire shape
		int rows = Mathf.CeilToInt( planeOutline.height / tileSize.y );
		
		for ( int iCol=0; iCol<columns; iCol++ ) {
			for ( int iRow=0; iRow<rows; iRow++ ) {
				Vector2 topLeft =     new Vector3(planeOutline.x + iCol*tileSize.x,      planeOutline.y + (iRow+1)*tileSize.y);
				Vector2 topRight =    new Vector3(planeOutline.x + (iCol+1)*tileSize.x,  planeOutline.y + (iRow+1)*tileSize.y);
				Vector2 bottomLeft =  new Vector3(planeOutline.x + iCol*tileSize.x,      planeOutline.y + iRow*tileSize.y);
				Vector2 bottomRight = new Vector3(planeOutline.x + (iCol+1)*tileSize.x,  planeOutline.y + iRow*tileSize.y);
				
				Shape2D tileShape = new Shape2D();
				tileShape.AddRange( new Vector2[]{ topLeft, topRight, bottomRight, bottomLeft } );
				
				Mesh mesh = new Mesh();
				
				if ( !shapeOutline.Contains( tileShape ) ) { // The tile is square, so no need for clipping
					mesh = GenerateClippedQuad( tileShape, shapeOutline, ClipType.ctDifference );
				}
				
				CombineInstance meshInstance = new CombineInstance();
				meshInstance.mesh = mesh;
				meshInstance.subMeshIndex = 0;
				meshInstance.transform = Matrix4x4.identity;
				
				meshInstances.Add( meshInstance );
			}
		}
		
		
		Mesh shapeMesh = new Mesh();
		shapeMesh.name = "shapeMesh";
		shapeMesh.CombineMeshes( meshInstances.ToArray(), true );
		shapeMesh.Optimize();
		shapeMesh.RecalculateNormals();
		
		return shapeMesh;
	}

	private static Mesh GenerateQuad( Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight ) {
		Mesh mesh = new Mesh();
		
		mesh.vertices = new Vector3[]{
			VectorEx.Vec2ToVec3( topLeft ),
			VectorEx.Vec2ToVec3( topRight ),
			VectorEx.Vec2ToVec3( bottomRight ),
			VectorEx.Vec2ToVec3( bottomLeft )
		};
		mesh.uv = new Vector2[]{
			topLeft,
			topRight,
			bottomRight,
			bottomLeft
		};
		mesh.triangles = new int[]{
			0,1,3,
			1,2,3
		};

		return mesh;
	}

	private static Mesh GenerateClippedQuad( Shape2D tileShape, Shape2D shapeOutline, ClipType clipType ) {

		// START CLIPPING

		List<IntPoint> tileIntPoints = new List<IntPoint>();
		List<IntPoint> shapeIntPoints = new List<IntPoint>();
		
		foreach( Vector2 point in tileShape.GetPoints() ) { // Tile outline
			tileIntPoints.Add( new IntPoint( (long)( point.x * CLIPPER_SCALE ), (long)( point.y * CLIPPER_SCALE ) ) );
		}
		
		foreach( Vector2 point in shapeOutline.GetPoints() ) { // Object shape outline
			shapeIntPoints.Add( new IntPoint( (long)( point.x * CLIPPER_SCALE ), (long)( point.y * CLIPPER_SCALE ) ) );
		}
		
		Clipper c = new Clipper();
		c.AddPath( tileIntPoints, PolyType.ptSubject, true );
		c.AddPath( shapeIntPoints, PolyType.ptClip, true );
		
		
		List<List<IntPoint>> solutions = new List<List<IntPoint>>();
		bool clippingSucceeded = !c.Execute( clipType, solutions, PolyFillType.pftNonZero, PolyFillType.pftNonZero );


		if ( solutions.Count != 1 ) { // Something went wrong, so skip this tile
			//Debug.LogWarning("No solution mesh");
			return new Mesh();
		}
		
		List<IntPoint> solutionMesh = solutions[0];

		// END CLIPPING

		// START TRIANGULATION

		List<TriangulationPoint> solutionMeshVertecies = new List<TriangulationPoint>();
		
		foreach ( IntPoint solutionMeshPoint in solutionMesh ) {
			solutionMeshVertecies.Add( new TriangulationPoint( (double)(solutionMeshPoint.X / CLIPPER_SCALE), (double)(solutionMeshPoint.Y / CLIPPER_SCALE) ) );
		}
		
		PointSet pointSet = new PointSet(solutionMeshVertecies);
		
		List<Vector3> vertecies = new List<Vector3>();
		List<Vector2> UVs = new List<Vector2>();
		List<int> tris = new List<int>();
		
		if ( solutionMeshVertecies.Count > 3 ) {
			try {
				P2T.Triangulate(pointSet);
				
				foreach( TriangulationPoint point in pointSet.Points ) {
					vertecies.Add( new Vector3( point.Xf, 0f, point.Yf ) );
					UVs.Add( new Vector2( point.Xf, point.Yf ) );
				}
				
				foreach( DelaunayTriangle triangle in pointSet.Triangles ) {
					tris.Add( pointSet.Points.IndexOf( triangle.Points[2] ) );
					tris.Add( pointSet.Points.IndexOf( triangle.Points[1] ) );
					tris.Add( pointSet.Points.IndexOf( triangle.Points[0] ) );
				}
			}catch{
				Debug.LogWarning("Triangulation error");
			}
			
		} else {
			vertecies.Add( new Vector3( solutionMeshVertecies[0].Xf, 0f, solutionMeshVertecies[0].Yf ));
			UVs.Add( new Vector2( solutionMeshVertecies[0].Xf, solutionMeshVertecies[0].Yf ) );
			
			vertecies.Add( new Vector3( solutionMeshVertecies[1].Xf, 0f, solutionMeshVertecies[1].Yf ));
			UVs.Add( new Vector2( solutionMeshVertecies[1].Xf, solutionMeshVertecies[1].Yf ) );
			
			vertecies.Add( new Vector3( solutionMeshVertecies[2].Xf, 0f, solutionMeshVertecies[2].Yf ));
			UVs.Add( new Vector2( solutionMeshVertecies[2].Xf, solutionMeshVertecies[2].Yf ) );
			
			tris.AddRange( new int[]{2, 1, 0} );
		}
		
		Mesh mesh = new Mesh();

		mesh.vertices = vertecies.ToArray();
		mesh.uv = UVs.ToArray();
		mesh.triangles = tris.ToArray();

		// END TRIANGULATION

		return mesh;
	}

}
