/* Playtomic Unity3d API
-----------------------------------------------------------------------
 Documentation is available at: 
 	https://playtomic.com/api/unity

 Support is available at:
 	https://playtomic.com/community 
 	https://playtomic.com/issues
 	https://playtomic.com/support has more options if you're a premium user

 Github repositories:
 	https://github.com/playtomic

You may modify this SDK if you wish but be kind to our servers.  Be
careful about modifying the analytics stuff as it may give you 
borked reports.

Pull requests are welcome if you spot a bug or know a more efficient
way to implement something.

Copyright (c) 2011 Playtomic Inc.  Playtomic APIs and SDKs are licensed 
under the MIT license.  Certain portions may come from 3rd parties and 
carry their own licensing terms and are referenced where applicable.
*/ 

using System;
using System.Text;

internal class Playtomic_Encode
{
	/*
	 *  Using new because micro mscorlib doesn't support static Create
	 * */
	public static string MD5(string input)
	{
        var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sb = new StringBuilder();
		
        for (var i = 0; i < data.Length; i++)
            sb.Append(data[i].ToString("x2"));
		
        return sb.ToString();
    }
	
	public static string Base64(string data)
	{
        var enc = new byte[data.Length];
        enc = System.Text.Encoding.UTF8.GetBytes(data);    
        return Convert.ToBase64String(enc);
	}
}