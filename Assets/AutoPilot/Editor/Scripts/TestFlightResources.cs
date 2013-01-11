using System.IO;
using UnityEngine;
using System.Reflection;
using UnityEditor;

public class TestFlightResources
{
	private static Stream GetResourceStream (string resourceName, Assembly assembly)
	{
		if (assembly == null)
		{
			assembly = Assembly.GetExecutingAssembly ();
		}
	
		return assembly.GetManifestResourceStream (resourceName);
	}
	
	private static Stream GetResourceStream (string resourceName)
	{
		return GetResourceStream (resourceName, null);
	}
	
	private static byte[] GetByteResource (string resourceName, Assembly assembly)
	{
		Stream byteStream = GetResourceStream (resourceName, assembly);
		byte[] buffer = new byte[byteStream.Length];
		byteStream.Read (buffer, 0, (int)byteStream.Length);
		byteStream.Close ();
	
		return buffer;
	}
	
	private static byte[] GetByteResource (string resourceName)
	{
		return GetByteResource (resourceName, null);
	}
	
	public static Texture GetTextureResource (string resourceName, Assembly assembly)
	{
		Texture2D texture = new Texture2D(4,4);
		texture.LoadImage (GetByteResource (resourceName, assembly));
	
		return texture;
	}
	
	public static Texture GetTextureResource (string resourceName)
	{
		Texture localTex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/AutoPilot/Editor/Images/"+resourceName, typeof(Texture));
		if(localTex)
			return localTex;
		else 
			return GetTextureResource (resourceName, null);
	}
	
	public static T GetResource<T>(string resourceName)  where T: UnityEngine.Object
	{
		T localTex = (T)AssetDatabase.LoadAssetAtPath("Assets/AutoPilot/Editor/Images/"+resourceName, typeof(T));
		if(localTex)
			return localTex;
		else 
			return null;
	}
	
	private static Texture testflightSDKLogo = null;
	public static Texture TestflightSDKLogo 
	{ 
		get 
		{
			if(testflightSDKLogo == null) 
				testflightSDKLogo = GetTextureResource("tfSDK.png");
				
			return testflightSDKLogo;
		}
	}
	
	private static Texture testflightLiveLogo = null;
	public static Texture TestflightLiveLogo 
	{ 
		get 
		{
			if(testflightLiveLogo == null) 
				testflightLiveLogo = GetTextureResource("tfLive.png");
				
			return testflightLiveLogo;
		}
	}	
	
	private static GUISkin skin = null;
	public static GUISkin Skin
	{
		get 
		{
			if(skin == null)
			{
#if UNITY_3_5
				skin = TestFlightResources.GetResource<GUISkin>("Skin.guiskin");
#else	
				// load the unity 4 skin
				var from = "Assets/AutoPilot/Editor/Images/Unity4Skin.guiskin.zip";
				var to = "Assets/AutoPilot/Editor/Images/Unity4Skin.guiskin";
				
				if(!File.Exists(to) && File.Exists(from))
				{
					var args = string.Format("-o \"{0}\" -d \"{1}\"", from, Path.GetDirectoryName(to));
					System.Diagnostics.Process zipProcess = System.Diagnostics.Process.Start("unzip", args);
					zipProcess.WaitForExit(500);
				}
				
				if(File.Exists(to))
				{
					UnityEditor.AssetDatabase.ImportAsset(to);
					skin = TestFlightResources.GetResource<GUISkin>("Unity4Skin.guiskin");
				}
#endif
			}
			
			return skin;
		}
	}
}