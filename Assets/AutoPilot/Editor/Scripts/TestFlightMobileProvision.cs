using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEditor;
using System.Security.Cryptography;

[Serializable]
public class CachedProvision
{
	public CachedProvision() {}
	public CachedProvision(TestFlightMobileProvision provision, byte[] hash)
	{
		this.provision = provision;
		this.hash = hash;
	}
	
	public TestFlightMobileProvision provision;
	public byte[]	hash;
}

[System.Serializable]
public class TestFlightMobileProvision : System.IEquatable<TestFlightMobileProvision>
{
	string uuid; 
	string name;
	string appIdentifier;
	string creationDate;
	bool   expired;
	
	public string 			Name 			{get {return this.name;} set {name = value;} }
	public string 			UUID 			{get {return this.uuid;} set {uuid = value;} }
	public bool 			Expired 		{get {return this.expired;} set {expired = value;} }	
	public string 			AppIdentifier 	{get {return this.appIdentifier;} set {appIdentifier = value;}}
	public DateTime 		CreationDate	{get {DateTime cd; if(!DateTime.TryParse(creationDate, out cd)) cd=DateTime.MinValue;  return cd;} }
	public string[]			Certificates 	{get; private set;}

	TestFlightMobileProvision()
	{
	}

	TestFlightMobileProvision(string data)
	{
		uuid = GetMobileProvisionUUID(data);
		expired = GetExpired(data);
		appIdentifier = GetAppId(data);
		creationDate = GetCreationDate(data);
		name = string.Format("{0} ({1})", GetMobileProvisionName(data), appIdentifier);
	}

	void VerifyIdentity (string data)
	{
		Match matchCertPack = Regex.Match(data,"<key>DeveloperCertificates</key>\\s*<array>((.|\n)*?)</array>");
		if(!matchCertPack.Success || matchCertPack.Groups.Count < 2)
		{
			Certificates = null;
			return;
		}
		
		string certificateKeys = matchCertPack.Groups[1].Value;
		var certificateNames = new List<string>();
		
		foreach(Match match in Regex.Matches(certificateKeys, "<data>((.|\n)*?)</data>"))
		{
			if(match.Groups.Count < 2)
				continue;
			
			string certKey = match.Groups[1].Value;
			byte[] bytes = null;
			try
			{
				bytes = Convert.FromBase64String(certKey);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
	
			var psi = new ProcessStartInfo("openssl", "x509 -subject -inform der");
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardInput = true;
			var proc = Process.Start(psi);
			
			proc.StandardInput.BaseStream.Write(bytes,0, bytes.Length);
			
			proc.StandardInput.Close();
			proc.WaitForExit();

			string output = proc.StandardOutput.ReadToEnd();
			Match matchId = Regex.Match(output, "/CN=([^/]+)");
			if(matchCertPack.Success && matchId.Groups.Count >= 2)
			{
				if(!certificateNames.Contains(matchId.Groups[1].Value))
					certificateNames.Add(matchId.Groups[1].Value);
			}
		}
		
		if(certificateNames.Count > 0)
			Certificates = certificateNames.ToArray();
		else 
			Certificates = null;
	}
	
	public bool MatchesBundleId(string bundleId)
	{
		int start = appIdentifier.IndexOf('.');
		if(start == -1)
			return false;
		
		string appId = appIdentifier.Substring(start+1);
		int max = System.Math.Min(appId.Length, bundleId.Length);
		for(int i=0; i<max; ++i)
		{
			if(appId[i] == '*')
				return true;
			
			if(appId[i] != bundleId[i])
				return false;
		}
		return true;
	}
	
	public bool Equals (TestFlightMobileProvision other)
	{
		return other!=null && name == other.name && uuid == other.uuid;
	}
	
	private string GetAppId(string data)
	{
		Match match = Regex.Match(data, "<key>application-identifier</key>.*\n.*<string>(.*)</string>");
		if(match==null || match.Groups.Count < 2)
			return "";
		
		return match.Groups[1].Value;
	}
	
	private string GetCreationDate(string data)
	{
		Match match = Regex.Match(data, "<key>CreationDate</key>.*\n.*<date>(.*)</date>");
		if(match==null || match.Groups.Count < 2)
			return "";
		
		return match.Groups[1].Value;
	}
	
	private bool GetExpired(string data)
	{
		Match match = Regex.Match(data, "<key>ExpirationDate</key>.*\n.*<date>(.*)</date>");
		if(match==null || match.Groups.Count < 2)
			return true;
		
		System.DateTime expiry = System.DateTime.Parse(match.Groups[1].Value); 
		return System.DateTime.Now > expiry;
	}
	
	private string GetMobileProvisionName(string data)
	{
		
		Match match = Regex.Match(data, "<key>Name</key>.*\n.*<string>(.*)</string>");
		if(match==null || match.Groups.Count < 2)
			return "";
		
		return match.Groups[1].Value; 
	}
	
	private string GetMobileProvisionUUID(string data)
	{
		Match match = Regex.Match(data, "<key>UUID</key>.*\n.*<string>(.*)</string>");
		if(match==null || match.Groups.Count < 2)
			return "";
		
		return match.Groups[1].Value; 
	}
	
	public static TestFlightMobileProvision[] EnumerateProvisions()
	{
		if(cache == null)
			cache = new TestFlightProvisionCache();
		return cache.OutputProvisions;
	}
	
	static TestFlightProvisionCache cache = null;
	public static string MobileProvisionFolder { get { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)+"/Library/MobileDevice/Provisioning Profiles/";} }
	
	[Serializable]
	private class TestFlightProvisionCache
	{ 
		private FileSystemWatcher watcher = new FileSystemWatcher(Path.GetFullPath(TestFlightMobileProvision.MobileProvisionFolder), "*.mobileprovision");
		
		public TestFlightMobileProvision[] OutputProvisions {get; private set;}
		
		public TestFlightProvisionCache()
		{
			RescanProvisions();
			watcher.Changed += delegate(object sender, FileSystemEventArgs e) 	{RescanProvisions();};
			watcher.Created += delegate(object sender, FileSystemEventArgs e) 	{RescanProvisions();};
			watcher.Deleted += delegate(object sender, FileSystemEventArgs e) 	{RescanProvisions();};
			watcher.Renamed += delegate(object sender, RenamedEventArgs e) 		{RescanProvisions();};
		}

		List<CachedProvision> LoadCache ()
		{
			List<CachedProvision> cachedProvisions = null;
			try
			{
				var cacheFileSream = new FileStream("TestFlightMobileProvisionCache.xml",FileMode.Open);
				var cacheDeseralizer = new System.Xml.Serialization.XmlSerializer(typeof(List<CachedProvision>));				
				cachedProvisions = (List<CachedProvision>)cacheDeseralizer.Deserialize(cacheFileSream);
				cacheFileSream.Close();
			}
			catch(Exception) {}		
			
			if(cachedProvisions == null) 
				cachedProvisions = new List<CachedProvision>();
			
			return cachedProvisions;
		}
		
		void SaveCache(List<CachedProvision> cachedProvisions)
		{
			var cacheFileSream = new FileStream("TestFlightMobileProvisionCache.xml",FileMode.Create);
			var cacheSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<CachedProvision>));				
			cacheSerializer.Serialize(cacheFileSream, cachedProvisions);
			cacheFileSream.Close();
		}
		
		private void RescanProvisions()
		{
			OutputProvisions = new TestFlightMobileProvision[0];
			try 
			{
				DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(TestFlightMobileProvision.MobileProvisionFolder));
		
				if(!dir.Exists)
					return;
		
				List<CachedProvision> 			cachedProvisions = LoadCache();
				List<TestFlightMobileProvision> loadedProvisions = new List<TestFlightMobileProvision>();
				
				string[] paths = Directory.GetFiles(dir.FullName, "*.mobileprovision");
				for(int i=0;i<paths.Length; ++i)
				{					
					var s = paths[i];
					
					StreamReader sr = new StreamReader(s);
			
					if(sr.EndOfStream)
						return;
			
					string data = sr.ReadToEnd();
					sr.Close();
					
					TestFlightMobileProvision p = new TestFlightMobileProvision(data);
					byte[] pHash = MD5.Create().ComputeHash(new FileStream(s, FileMode.Open));
							
					loadedProvisions.Add(p);
					
					if(ShouldUseCached(cachedProvisions, p, pHash))
						continue;
					
					EditorUtility.DisplayProgressBar("Verifying provisions & developer identities", p.Name, (float)i/paths.Length);
	
					// verify and add to cache
					p.VerifyIdentity(data);
					cachedProvisions.RemoveAll(m => m.provision.Equals(p));
					cachedProvisions.Add(new CachedProvision(p, pHash));
				}
				
				// remove any missing provisions
				cachedProvisions.RemoveAll(m => !loadedProvisions.Exists(n => m.provision.Equals(n)));
				
				// add all cached provisons
				List<TestFlightMobileProvision> provisions = new List<TestFlightMobileProvision>();
				foreach(var c in cachedProvisions)
				{
					if(c.provision.Expired)
						continue;
						
					provisions.Add(c.provision);
				}
				
				SaveCache(cachedProvisions);
	
				OutputProvisions = provisions.ToArray();
			}
			catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
			
			EditorUtility.ClearProgressBar();
		}

		bool ShouldUseCached(List<CachedProvision> cachedProvisions, TestFlightMobileProvision p, byte[] pHash)
		{
			CachedProvision cached = cachedProvisions.Find(delegate(CachedProvision obj) {return obj.provision.Equals(p);});
			if(cached == null)
				return false;
						
			if(cached.provision == null)
				return false;
			
			if(cached.provision.Certificates == null)				
				return false;
			
			if(cached.hash == null || pHash.Length != cached.hash.Length)
				return false;
			
			if(p.CreationDate <= cached.provision.CreationDate)
				return false;
				
			int index=0; 
			while(index<pHash.Length && pHash[index] == cached.hash[index])
			{
				++index;
			}
				
			return (index==pHash.Length);
		}
	}
}