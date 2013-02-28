/* Playtomic ActionScript 3 API
-----------------------------------------------------------------------
Note:  This requires a Playtomic.com account AND a Parse.com account,
you will have to register at Parse and configure the settings in your
Playtomic dashboard.  Your Parse account and billing are completely
separate to your Playtomic account.  Parse have native support for a
growing number of libraries, possibly including this:

 Parse:
	https://parse.com/
	
 Documentation is available at: 
 	https://playtomic.com/api/as3

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
using System.Collections.Generic;

public class PFObject
{
	public PFObject () { }

	public string ObjectId;
	public string ClassName;
	public List<PFPointer> Pointers = new List<PFPointer>();
	public Dictionary<string, string> Data = new Dictionary<string, string>();
	public DateTime UpdatedAt;
	public DateTime CreatedAt;
	public String Password;
}
