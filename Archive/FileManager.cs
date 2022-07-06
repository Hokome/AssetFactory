using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEditor;
using UnityEngine;

namespace AssetFactory.IO
{
	public static class FileManager
	{
		public static void WriteFileBytes(string path, byte[] data)
		{
			if (!File.Exists(path))
			{
				File.Create(path);
			}
			File.WriteAllBytes(path, data);
		}
		public static void WriteFileBytes<T>(string path, T data) where T : IByteConvertible
			=> WriteFileBytes(path, data.ToBytes());

		public static void WriteFile<T>(string path, T data) where T : IFileConvertible
		{
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
			data.ToFile(fs);
			fs.Close();
		}
		public static T ReadFileBytes<T>(string path, ref T inst) where T : IByteConvertible
		{
			if (!File.Exists(path))
				return default;
			inst.FromBytes(File.ReadAllBytes(path));
			return inst;
		}
		public static T ReadFile<T>(string path, ref T inst) where T : IFileConvertible
		{
			if (!File.Exists(path))
				return inst;
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
			inst.FromFile(fs);
			fs.Close();
			return inst;
		}

		//public static T ReadFileJson<T>(string path) where T : IJsonConvertible
		//{
		//	T t = default;
		//	if (!File.Exists(path))
		//		return t;

		//	string s = File.ReadAllText(path);
		//	return t.FromJson(s);
		//}
	}
}