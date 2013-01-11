using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class TestFlightDeveloperIdentities
{
/*
	[UnityEditor.MenuItem("Test/Enumerate Identities")]
	public static void EnumerateTest()
	{
		foreach(var s in Enumerate())
			UnityEngine.Debug.Log(s);
	}
*/	
	public static string[] Enumerate()
	{
		List<string> identities = new List<string>();

		try
		{
			System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo("security", "-v find-identity");
			pi.RedirectStandardOutput = true;
			pi.RedirectStandardError = true;
			pi.UseShellExecute = false;
			System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
			if(p == null)
				return null;
			
			p.WaitForExit();		
			if(p.ExitCode != 0)
				return null;
	
			foreach(Match m in Regex.Matches(p.StandardOutput.ReadToEnd(), "\"(iPhone.*)\""))
			{
				for(int i=1; i<m.Groups.Count; ++i)
				{
					foreach(Capture c in m.Groups[i].Captures)
						identities.Add(c.Value);
				}
			}
		}
		catch(System.Exception e)
		{
			UnityEngine.Debug.LogError(e);
		}
		
		return identities.ToArray();
	}
}