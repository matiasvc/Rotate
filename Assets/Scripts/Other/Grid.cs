using UnityEngine;

public class Grid{
	
	public Grid(float left, float top, float width, float height, int verticalNumber, int horizontalNumber)
	{
		gridArea = new Rect (left, top, width, height);
		
		gridVerNum = verticalNumber;
		gridHorNum = horizontalNumber;
		
		CalculatePositions();
	}
	
	public Grid(Rect gridRect, int verticalNumber, int horizontalNumber)
	{
		gridArea = gridRect;
		
		gridVerNum = verticalNumber;
		gridHorNum = horizontalNumber;
		
		CalculatePositions();
	}
	
	#region fields
	
	private Rect gridArea;
	
	private int gridVerNum;
	private int gridHorNum;
	
	private float[] verPoints;
	private float[] horPoints;
	
	private Vector2[] nodePoints;
	
	#endregion
	
	
	#region properties
	
	public Rect GridArea
	{
		get { return gridArea; }
		set
		{
			gridArea = value;
			CalculatePositions();
		
		}
	}
	
	public int VerticalNumber
	{
		get { return gridVerNum; }
		set { gridVerNum = value; }
	}
	
	public int HorizontalNumber
	{
		get { return gridHorNum; }
		set { gridHorNum = value; }
	}
	
	public int NumberOfGridNodes
	{
		get { return nodePoints.Length; }
	}
	
	#endregion
	
	
	#region methods
	
	public Vector2 GetNodePostion(int node)
	{
		if(node > (nodePoints.Length - 1))
		{
			Debug.LogError("Grid node " + node + " does not exist");
			
			return Vector2.zero;	
		}
		else
		{
			return nodePoints[node];
		}
	}
	
	private void CalculatePositions()
	{
		verPoints = new float[gridVerNum];
		horPoints = new float[gridHorNum];
		
		nodePoints = new Vector2[gridVerNum * gridHorNum];
		
		for(int i = 0; i <= (gridVerNum - 1); i++)
		{
			verPoints[i] = Mathf.Lerp(gridArea.y, gridArea.y - gridArea.height, i / ((float)gridVerNum - 1.0f));
			//Debug.Log(verPoints[i]);
		}
		
		for(int i = 0; i <= (gridHorNum - 1); i++)
		{
			horPoints[i] = Mathf.Lerp(gridArea.x, gridArea.x + gridArea.width, i / ((float)gridHorNum - 1.0f));
			//Debug.Log(horPoints[i]);
		}
		
		int currentNode = 0;
		
		for(int iVert = 0; iVert <= (gridVerNum - 1); iVert++)
		{
			for(int iHor = 0; iHor <= (gridHorNum - 1); iHor++)
			{
				nodePoints[currentNode] = new Vector2(horPoints[iHor], verPoints[iVert]);
				currentNode++;
			}
		}
	}
	
	#endregion
}
