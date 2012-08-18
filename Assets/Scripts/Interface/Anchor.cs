using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Anchor : MonoBehaviour {
	
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center,
	}
	
	public Camera uiCamera = null;
	public Side side = Side.Center;
	public float depthOffset = 0f;
	public Vector2 relativeOffset = Vector2.zero;
	
	void Update()
	{
		if (uiCamera != null)
		{
			Rect rect = uiCamera.pixelRect;
			float cx = (rect.xMin + rect.xMax) * 0.5f;
			float cy = (rect.yMin + rect.yMax) * 0.5f;
			Vector3 v = new Vector3(cx, cy, depthOffset);

			if (side != Side.Center)
			{
				if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
				{
					v.x = rect.xMax;
				}
				else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
				{
					v.x = cx;
				}
				else
				{
					v.x = rect.xMin;
				}

				if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
				{
					v.y = rect.yMax;
				}
				else if (side == Side.Left || side == Side.Center || side == Side.Right)
				{
					v.y = cy;
				}
				else
				{
					v.y = rect.yMin;
				}
			}

			float screenWidth  = rect.width;
			float screenHeight = rect.height;

			v.x += relativeOffset.x * screenWidth;
			v.y += relativeOffset.y * screenHeight;

			if (uiCamera.orthographic)
			{
				v.x = Mathf.RoundToInt(v.x);
				v.y = Mathf.RoundToInt(v.y);
			}

			// Convert from screen to world coordinates, since the two may not match (UIRoot set to manual size)
			v = uiCamera.ScreenToWorldPoint(v);

			// Wrapped in an 'if' so the scene doesn't get marked as 'edited' every frame
			if (transform.position != v) transform.position = v;
		}
	}
}
