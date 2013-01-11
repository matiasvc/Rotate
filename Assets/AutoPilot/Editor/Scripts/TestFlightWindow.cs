using UnityEngine;
using UnityEditor;

public class TestFlightWindow : EditorWindow
{
	protected GUISkin skin;
	
	protected void InitSkin()
	{
		if(skin == null)
			skin = TestFlightResources.Skin;
		
		if(skin)
		{
			GUI.skin = skin;
			EditorGUIUtility.LookLikeControls();
			GUI.Box(new Rect(0,0, Screen.width, Screen.height), "", "hostview"); 
		}
	}
}