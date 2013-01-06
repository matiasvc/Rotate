using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

public class ZipUtil : MonoBehaviour {
	
	public static byte[] CompressBZip2(byte[] data)
	{
		MemoryStream memoryStream = null;
		BZip2OutputStream zipOutputSteam = null;
		byte[] result;
		
		try
		{
			memoryStream = new MemoryStream();
			Int32 size = data.Length;
			// Prepend the compressed data with the length of the uncompressed data (first 4 bytes)
			using (BinaryWriter writer = new BinaryWriter(memoryStream))
			{
				writer.Write(size);
				
				zipOutputSteam = new BZip2OutputStream(memoryStream);
				zipOutputSteam.Write(data, 0, data.Length);
				
				zipOutputSteam.Close();
				result = memoryStream.ToArray();
				memoryStream.Close();
				
				writer.Close();
			}
		}
		finally
		{
			if (zipOutputSteam != null)
				zipOutputSteam.Dispose();
			if (memoryStream != null)
				memoryStream.Dispose();
		}
		
		return result;
	}
	
	public static byte[] UncompressBZip2(byte[] data)
	{
		MemoryStream memoryStream = null;
		BZip2InputStream zipInputStream = null;
		byte[] result;
		
		try
		{
			memoryStream = new MemoryStream(data);
			// read final uncompressed string size stored in first 4 bytes
			using (BinaryReader reader = new BinaryReader(memoryStream))
			{
				Int32 size = reader.ReadInt32();
				
				zipInputStream = new BZip2InputStream(memoryStream);
				result = new byte[size];
				zipInputStream.Read(result, 0, result.Length);
				
				zipInputStream.Close();
				memoryStream.Close();
				reader.Close();
			}
		}
		finally
		{
			if (zipInputStream != null)
				zipInputStream.Dispose();
			if (memoryStream != null)
				memoryStream.Dispose();
		}
		return result;
	}
	
	public static byte[] CompressGZip(byte[] data)
	{
		MemoryStream memoryStream = null;
		GZipOutputStream zipOutputSteam = null;
		byte[] result;
		
		try
		{
			memoryStream = new MemoryStream();
			Int32 size = data.Length;
			// Prepend the compressed data with the length of the uncompressed data (first 4 bytes)
			using (BinaryWriter writer = new BinaryWriter(memoryStream))
			{
				writer.Write(size);
				
				zipOutputSteam = new GZipOutputStream(memoryStream);
				zipOutputSteam.Write(data, 0, data.Length);
				
				zipOutputSteam.Close();
				result = memoryStream.ToArray();
				memoryStream.Close();
				
				writer.Close();
			}
		}
		finally
		{
			if (zipOutputSteam != null)
				zipOutputSteam.Dispose();
			if (memoryStream != null)
				memoryStream.Dispose();
		}
		
		return result;
	}
	
	public static byte[] UncompressGZip(byte[] data)
	{
		MemoryStream memoryStream = null;
		GZipInputStream zipInputStream = null;
		byte[] result;
		
		try
		{
			memoryStream = new MemoryStream(data);
			// read final uncompressed string size stored in first 4 bytes
			using (BinaryReader reader = new BinaryReader(memoryStream))
			{
				Int32 size = reader.ReadInt32();
				
				zipInputStream = new GZipInputStream(memoryStream);
				result = new byte[size];
				zipInputStream.Read(result, 0, result.Length);
				
				zipInputStream.Close();
				memoryStream.Close();
				reader.Close();
			}
		}
		finally
		{
			if (zipInputStream != null)
				zipInputStream.Dispose();
			if (memoryStream != null)
				memoryStream.Dispose();
		}
		return result;
	}
}
