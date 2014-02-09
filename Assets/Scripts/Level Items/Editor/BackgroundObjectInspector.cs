using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;
using Poly2Tri;

[CustomEditor(typeof(BackgroundObjectController))]
public class BackgroundObjectInspector : Editor {

	public enum Type { PLANE, SHAPE }

	private BackgroundObjectController _editorTarget = null;
	private BackgroundObjectController editorTarget {
		get {
			if (_editorTarget == null) {
				_editorTarget = target as BackgroundObjectController;
			}
			return _editorTarget;
		}
	}

	private const float clipperScale = 10000f;

	private Type type = Type.PLANE;

	// Plane variables
	private Rect _planeOutline = new Rect( -5f, -5f, 10f, 10f );
	private List<Shape2D> _planeHoles = null;
	// Shape variables
	public Shape2D shapeOutline;

	public override void OnInspectorGUI () {

		editorTarget.animationSpeed = EditorGUILayout.Vector2Field( "UV Animation:", editorTarget.animationSpeed );
		editorTarget.meshRenderer.sharedMaterial = EditorGUILayout.ObjectField( editorTarget.meshRenderer.sharedMaterial, typeof(Material), false ) as Material;

		GUILayout.Space( 15f );
		GUILayout.Label( "MESH" );

		editorTarget.tileSize = EditorGUILayout.Vector2Field( "Tile Size:", editorTarget.tileSize );

		//type = (Type)EditorGUILayout.EnumPopup( "Type", type );

		if ( type == Type.PLANE ) {

		} else {

		}

		if (GUILayout.Button( "Generate Mesh" )) {
			GenerateMesh();
		}

	}

	void OnEnable() {
		if( editorTarget.type != 0 ) { type = Type.SHAPE; } else { type = Type.PLANE; }

		if (string.IsNullOrEmpty(editorTarget.shapeData) || editorTarget.shapeData == "null") {
			shapeOutline = Shape2D.triangle;
		} else {
			JSONObject shapeObj = new JSONObject(editorTarget.shapeData);

			shapeOutline = new Shape2D();
			foreach (JSONObject vecObj in shapeObj.list) {
				Vector2 point = new Vector2( (float)vecObj.GetField("x").n, (float)vecObj.GetField("y").n );
				shapeOutline.AddPoint(point);
			}
		}
	}

	void OnDestroy() { // Serialize the shape
		editorTarget.type = (int)type;

		JSONObject shapeObj = new JSONObject(JSONObject.Type.ARRAY);
		Vector2[] shapePoints = shapeOutline.GetPoints();

		foreach (Vector2 vec2 in shapePoints) {
			JSONObject vecObj = new JSONObject(JSONObject.Type.OBJECT);
			vecObj.AddField("x", vec2.x);
			vecObj.AddField("y", vec2.y);
			shapeObj.Add(vecObj);
		}

		editorTarget.shapeData = shapeObj.print();
		EditorUtility.SetDirty( editorTarget );
	}

	void OnSceneGUI() {

		//if ( type == Type.PLANE ) {

		//} else { // Render lines and edit spheres for the shape

			Line2D[] outline = shapeOutline.GetOutline();

			Transform targetTransform = editorTarget.transform;

			for (int i=0; i<outline.Length; i++) {
				Line2D line = outline[i];
				Vector3 start = targetTransform.TransformPoint( VectorEx.Vec2ToVec3( line.start ) );
				Vector3 end = targetTransform.TransformPoint( VectorEx.Vec2ToVec3( line.end ) );
				Handles.color = Color.white;
				Handles.DrawLine(start, end);

				Handles.color = Color.green;
				Vector3 buttonPos = Vector3.Lerp(start, end, 0.5f);
				if (Handles.Button( buttonPos, targetTransform.rotation, 0.15f, 0.2f, Handles.SphereCap )) {
					shapeOutline.InsertPointAtIndex(VectorEx.Vec3ToVec2(targetTransform.InverseTransformPoint(buttonPos)), i+1);
				}
			}

			Vector2[] points = shapeOutline.GetPoints();
			for (int i=0; i<points.Length; i++) {
				Vector3 worldPos = targetTransform.TransformPoint(VectorEx.Vec2ToVec3(points[i]));

				Handles.color = Color.blue;
				Vector3 newPos = Handles.Slider2D( worldPos, targetTransform.up, targetTransform.forward, targetTransform.right, 0.25f, Handles.SphereCap, 1f );
				Vector3 deleteButtonPos = worldPos + QuaternionEx.GetRightVector(SceneView.lastActiveSceneView.rotation) * 0.15f + QuaternionEx.GetUpVector(SceneView.lastActiveSceneView.rotation) * 0.15f;

				newPos = targetTransform.InverseTransformPoint(newPos);
				Vector2 newPos2D = VectorEx.Vec3ToVec2(newPos);

				if (!Mathf.Approximately( points[i].x, newPos2D.x) ||
				    !Mathf.Approximately( points[i].y, newPos2D.y) ) {
					shapeOutline.SetPointAtIndex(newPos2D, i);
				}

				Handles.color = Color.red;
				if (Handles.Button( deleteButtonPos, targetTransform.rotation, 0.05f, 0.05f, Handles.DotCap )) {
					shapeOutline.RemovePointAtIndex(i);
				}
			}
		//}

	}


	private void GenerateMesh() {

		Transform targetTransform = editorTarget.transform;
		Vector2 tileSize = editorTarget.tileSize;
		Rect meshBounds = shapeOutline.GetBounds();

		int collums = Mathf.CeilToInt( meshBounds.width / tileSize.x );
		int rows = Mathf.CeilToInt( meshBounds.height / tileSize.y );

		List<CombineInstance> meshInstances = new List<CombineInstance>();

		for (int iCol=0; iCol<collums; iCol++) {
			for (int iRow=0; iRow<rows; iRow++) {
				Rect tileRect = new Rect(meshBounds.x + iCol*tileSize.x, meshBounds.y + iRow*tileSize.y, tileSize.x, tileSize.y);

				Vector2 topLeft =     new Vector3(tileRect.xMin, tileRect.yMax);
				Vector2 topRight =    new Vector3(tileRect.xMax, tileRect.yMax);
				Vector2 bottomLeft =  new Vector3(tileRect.xMin, tileRect.yMin);
				Vector2 bottomRight = new Vector3(tileRect.xMax, tileRect.yMin);

				Shape2D tileShape = new Shape2D();
				tileShape.AddRange( new Vector2[]{ topLeft, topRight, bottomRight, bottomLeft } );

				Mesh mesh = new Mesh();

				if ( shapeOutline.Contains( tileShape ) ) { // The tile is square, so no need for clipping

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

				} else { // The tile is somehow clipped, so we need to involve the Clipper and Poly2Tri libs

					// START CLIPPING
					List<IntPoint> tileIntPoints = new List<IntPoint>();
					List<IntPoint> shapeIntPoints = new List<IntPoint>();

					foreach( Vector2 point in tileShape.GetPoints() ) { // Tile outline
						tileIntPoints.Add( new IntPoint( (long)( point.x * clipperScale ), (long)( point.y * clipperScale ) ) );
					}

					foreach( Vector2 point in shapeOutline.GetPoints() ) { // Object shape outline
						shapeIntPoints.Add( new IntPoint( (long)( point.x * clipperScale ), (long)( point.y * clipperScale ) ) );
					}

					Clipper c = new Clipper();
					c.AddPath( tileIntPoints, PolyType.ptSubject, true );
					c.AddPath( shapeIntPoints, PolyType.ptClip, true );


					List<List<IntPoint>> solutions = new List<List<IntPoint>>();
					bool clippingSucceeded = !c.Execute( ClipType.ctIntersection, solutions, PolyFillType.pftNonZero, PolyFillType.pftNonZero );

					if ( solutions.Count != 1 ) { // Something went wrong, so skip this tile
						continue;
					}

					List<IntPoint> solution = solutions[0];

					// END CLIPPING

					// START TRIANGULATION
					foreach( List<IntPoint> solutionMesh in solutions ) {
						List<TriangulationPoint> solutionMeshVertecies = new List<TriangulationPoint>();

						foreach ( IntPoint solutionMeshPoint in solutionMesh ) {
							solutionMeshVertecies.Add( new TriangulationPoint( (double)(solutionMeshPoint.X / clipperScale), (double)(solutionMeshPoint.Y / clipperScale) ) );
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

						
						mesh.vertices = vertecies.ToArray();
						mesh.uv = UVs.ToArray();
						mesh.triangles = tris.ToArray();
					}
					// EMD TRIANGULATION
				}

				CombineInstance meshInstance = new CombineInstance();
				meshInstance.mesh = mesh;
				meshInstance.subMeshIndex = 0;
				meshInstance.transform = Matrix4x4.identity;
				
				meshInstances.Add( meshInstance );
				
			} // Tile row loop
		} // Tile colum loop

		Mesh shapeMesh = new Mesh();
		shapeMesh.name = "shapeMesh";
		shapeMesh.CombineMeshes( meshInstances.ToArray(), true );
		shapeMesh.Optimize();
		shapeMesh.RecalculateNormals();

		editorTarget.meshFilter.mesh = shapeMesh;

	}
}
