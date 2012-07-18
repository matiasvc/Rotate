using UnityEngine;
using System.Collections;

public class HUDFPS : MonoBehaviour
{
	public enum Placement
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
	
	public  float updateInterval = 0.5f;
	public Placement placement = Placement.TopRight;
	
	private float accum   = 0;
	private int   frames  = 0;
	private float timeleft;
	
	private string fpsString = "";
	private Color color;
	
	void Start()
	{
	    timeleft = updateInterval;  
	}
	 
	void Update()
	{
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
		
	    if (timeleft <= 0.0 )
	    {
		    float fps = accum/frames;
		    fpsString = System.String.Format("{0:F2} FPS",fps);
		
		    if (fps < 30)
		        color = Color.yellow;
		    else
			{
				if (fps < 15)
					color = Color.red;
				else
					color = Color.green;				
			}
			
	        timeleft = updateInterval;
	        accum = 0.0f;
	        frames = 0;
	    }
	}
	
	void OnGUI ()
	{
		Color oldColor = GUI.contentColor;
		GUI.contentColor = color;
		
		if (placement == Placement.TopRight)
			GUI.Label (new Rect (Screen.width - 50, 5, 50, 20), fpsString);
		else if (placement == Placement.TopLeft)
			GUI.Label (new Rect (5, 5, 50, 20), fpsString);
		else if (placement == Placement.BottomLeft)
			GUI.Label (new Rect (5, Screen.height - 20, 50, 20), fpsString);
		else if (placement == Placement.BottomLeft)
			GUI.Label (new Rect (Screen.width - 50, Screen.height - 20, 50, 20), fpsString);
		
		GUI.contentColor = oldColor;
	}
}