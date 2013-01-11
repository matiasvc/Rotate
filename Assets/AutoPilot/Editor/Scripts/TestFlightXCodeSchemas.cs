using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TestFlightXCodeSchemas
{
	List<string> 	targets = new List<string>();
	List<string> 	configs = new List<string>();
	List<string> 	schemes = new List<string>();
	int				defaultConfig = 0;
	
	public string[] Targets { get { return targets.ToArray(); } }
	public string[] Configs { get { return configs.ToArray(); } }
	public string[] Schemes { get { return schemes.ToArray(); } }
	public string DefaultConfig { get { return defaultConfig >= 0 && defaultConfig < configs.Count ? configs[defaultConfig]:"";} }
	
	/// <summary>
	/// protected constructor, use enumerate to create this type
	/// </summary>
	protected TestFlightXCodeSchemas() {}
	
	/// <summary>
	/// Enumerate the schemas for an xcode project
	/// </summary>
	/// <param name="projectPath">
	/// A <see cref="System.String"/> that contains the path to folder containing the xcode project (not the project itself)
	/// </param>
	/// <returns>
	/// A <see cref="TestFlightXCodeSchemas"/> containing the targets, configs and schemes belonging to that project. Or null on any error
	/// </returns>
	public static TestFlightXCodeSchemas Enumerate(string projectPath)
	{
		TestFlightXCodeSchemas output = new TestFlightXCodeSchemas();
		
		try 
		{
			System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo("xcodebuild", "-list");
			pi.WorkingDirectory = projectPath;
			pi.RedirectStandardOutput = true;
			pi.RedirectStandardError = true;
			pi.UseShellExecute = false;
			System.Diagnostics.Process p = System.Diagnostics.Process.Start(pi);
			if(p == null)
				return null;
			
			p.WaitForExit(2000);		
			if(!p.HasExited)
				return null;
			
			if(p.ExitCode != 0)
				return null;
			
			string strOutput = p.StandardOutput.ReadToEnd() + "\n";
			
			// build configs 
			output.targets = ExtractList("Targets", strOutput);
			output.configs = ExtractList("Build Configurations", strOutput);
			output.schemes = ExtractList("Schemes", strOutput);
			
			output.defaultConfig = output.configs.IndexOf(FindDefaultConfig(strOutput));
		}
		catch(System.Exception e) 
		{
			Debug.LogWarning("Autopilot: Unable to detect schema settings, reason:\n"+e);
		}
		
		return output;
	}
	
	static private List<string> ExtractList(string header, string input)
	{
		List<string> output = new List<string>();
		string pattern = @"(?:"+header+@"\:\n)(?:\s+(\S+)\n)+";
				
		Match m = Regex.Match(input, pattern);
		for(int i=1; i<m.Groups.Count; ++i)
		{
			foreach(Capture c in m.Groups[i].Captures)
				output.Add(c.Value);
		}
		return output;
	}
	
	static private string FindDefaultConfig(string input)
	{
		Match m = Regex.Match(input, "If no build configuration is specified and -scheme is not passed then \"([^\"]+)\" is used.");
		if(!m.Success || m.Groups.Count <= 1)
			return "";
		else 
			return m.Groups[1].Captures[0].Value;
	}
}