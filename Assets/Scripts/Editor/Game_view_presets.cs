using UnityEngine;
using UnityEditor;
using System.Reflection;

public class Game_view_presets : EditorWindow
{
	public static void NotifyScripts()
	{
		//Find all methods on scene scripts tagged SpriteSetUpdateCallback and call them.
		Transform[] __objects = GameObject.FindSceneObjectsOfType(typeof(Transform)) as Transform[];
		
		for (int i = 0; i < __objects.Length; i++)
		{
			MonoBehaviour[] __scripts = __objects[i].GetComponents<MonoBehaviour>();
			
			foreach(MonoBehaviour __script in __scripts)
			{
				if (__script.enabled == false)
				{
					continue;
				}
				
				System.Type __type = __script.GetType();
				
				MethodInfo[] __methods = __type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				
				foreach(MethodInfo __mi in __methods)
				{
					if (__mi.Name == "OnGameViewPresetChanged")
					{
						ParameterInfo[] __params = __mi.GetParameters();
						
						if (__params.Length == 2
							&& __params[0].ParameterType == typeof(int)
							&& __params[1].ParameterType == typeof(int))
						{
							__mi.Invoke(__script, new[]{(object)PlayerSettings.defaultScreenWidth, (object)PlayerSettings.defaultScreenHeight});
						}
					}
				}
			}
		}
		
		RepaintGameViews();
	}
	
	
    /*
	[SerializeField]
	string _preference = "";
	
	[SerializeField]
	float _floatValue = 0.0f;
	
	[SerializeField]
	int _intValue = 0;
	
	[SerializeField]
	string _stringValue = "";
	*/
	[MenuItem("Utils/Game View Presets")]
	private static void Init()
	{
        EditorWindow.GetWindow<Game_view_presets>();
	}
	
	protected void OnEnable()
	{
	}
	
	protected void OnDisable()
	{
	}
	
	
	
	protected void OnGUI()
	{
		GUILayout.BeginVertical();


        GUILayout.BeginHorizontal();
        GUILayout.Label("iPhone", GUILayout.Width(60));
        if (GUILayout.Button("480 x 320", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 480;
            PlayerSettings.defaultScreenHeight = 320;
            PlayerSettings.defaultWebScreenWidth = 480;
            PlayerSettings.defaultWebScreenHeight = 320;
			
			NotifyScripts();
        }
        if (GUILayout.Button("320 x 480", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 320;
            PlayerSettings.defaultScreenHeight = 480;
            PlayerSettings.defaultWebScreenWidth = 320;
            PlayerSettings.defaultWebScreenHeight = 480;
			
			NotifyScripts();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("iPhone 4", GUILayout.Width(60));
        if (GUILayout.Button("960 x 640", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 960;
            PlayerSettings.defaultScreenHeight = 640;
            PlayerSettings.defaultWebScreenWidth = 960;
            PlayerSettings.defaultWebScreenHeight = 640;
			
			NotifyScripts();
        }
        if (GUILayout.Button("640 x 960", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 640;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.defaultWebScreenWidth = 640;
            PlayerSettings.defaultWebScreenHeight = 960;
			
			NotifyScripts();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("iPad", GUILayout.Width(60));
        if (GUILayout.Button("1024 x 768", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 1024;
            PlayerSettings.defaultScreenHeight = 768;
            PlayerSettings.defaultWebScreenWidth = 1024;
            PlayerSettings.defaultWebScreenHeight = 768;
			
			NotifyScripts();
        }
        if (GUILayout.Button("768 x 1024", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 768;
            PlayerSettings.defaultScreenHeight = 1024;
            PlayerSettings.defaultWebScreenWidth = 768;
            PlayerSettings.defaultWebScreenHeight = 1024;
			
			NotifyScripts();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Facebook", GUILayout.Width(60));
        if (GUILayout.Button("544 x 408", GUILayout.Width(100))) {
            PlayerSettings.defaultScreenWidth = 544;
            PlayerSettings.defaultScreenHeight = 408;
            PlayerSettings.defaultWebScreenWidth = 544;
            PlayerSettings.defaultWebScreenHeight = 408;
			
			NotifyScripts();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
	}
	
	private static void RepaintGameViews()
	{
		Assembly __assembly = Assembly.GetAssembly(typeof(Editor));
		
		System.Type __gameViewType = __assembly.GetType("UnityEditor.GameView");
		
		if (__gameViewType == null)
		{
			Debug.LogWarning("Unable to find GameView");
		}
		else
		{
			UnityEngine.Object[] __windows = Resources.FindObjectsOfTypeAll(__gameViewType);
			
			if (__windows.Length == 0)
			{
				Debug.LogWarning("No GameView windows found");
			}
			else
			{
				for (int i = 0; i < __windows.Length; i++)
				{
					EditorWindow __window = __windows[i] as EditorWindow;
					
					__window.Repaint();
				}
				
				
			}
		}
	}
}


