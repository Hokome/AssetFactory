using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory
{
	public class SaveManager : MonoBehaviour
    {
		private static string installPath;
		private static string persistentPath;
		public static string SavePath { get; private set; }

		public static void SetPathToInstall()
		{
			if (!Directory.Exists(installPath))
				Directory.CreateDirectory(installPath);
			SavePath = installPath;
		}
		public static void SetPathToPersistent() => SavePath = persistentPath;
		public static void SaveFile(string localPath, ISaveable data)
		{
			if (data == null) throw new NullReferenceException("Saveable data is null");

			BinaryWriter writer = new(File.Create(SavePath + localPath));
			writer.Write(Application.version);
			data.SaveFile(writer);
			writer.Close();
		}
		public static void LoadFile(string localPath, ref ISaveable obj)
		{
			if (obj == null)
				throw new NullReferenceException("Saveable object must be initialized before loading from a file");

			string path = SavePath + localPath;
			if (!File.Exists(path))
				throw new FileNotFoundException($"Save was not found with provided path {SavePath + localPath}");

			ReadFile(path, ref obj);
		}
		public static bool TryLoadFile(string localPath, ref ISaveable obj)
		{
			string path = SavePath + localPath;
			if (!File.Exists(path))
				return false;

			if (obj == null)
				throw new NullReferenceException("Saveable object must be initialized before loading from a file");

			ReadFile(path, ref obj);
			return true;
		}
		private static void ReadFile(string fullPath, ref ISaveable obj)
		{
			BinaryReader reader = new(File.OpenRead(fullPath));
			string version = reader.ReadString();
			obj.LoadFile(reader);
			reader.Close();
		}
		//public static BinaryReader StartLookUp(string localPath)
		//{
		//	string path = SavePath + localPath;
		//	if (!File.Exists(path))
		//		throw new FileNotFoundException($"Save was not found with provided path {SavePath + localPath}");

		//	BinaryReader reader = new(File.OpenRead(localPath));
		//	string version = reader.ReadString();
		//	return reader;
		//}
		//public static void EndLookUp(BinaryReader reader) => reader.Close();
		//public static BinaryWriter StartEdit(string localPath)
		//{
		//	string path = SavePath + localPath;
		//	if (!File.Exists(path))
		//		throw new FileNotFoundException($"Save was not found with provided path {SavePath + localPath}");

		//	BinaryWriter writer = new(File.OpenWrite(SavePath + localPath));
		//	writer.Write(Application.version);
		//	return writer;
		//}
		private void Start()
		{
			installPath = @$"{Application.dataPath}/Save/";
			persistentPath = @$"{Application.persistentDataPath}/";
			SetPathToPersistent();
			Directory.CreateDirectory(installPath);
			Directory.CreateDirectory(persistentPath);
		}

		//public static bool TryLoadFile(string localPath, ref ISaveable obj, ref string version)
		//{
		//	if (obj == null) 
		//		throw new NullReferenceException("Saveable object must be initialized before loading from a file");

		//	string path = SavePath + localPath;
		//	if (!File.Exists(path)) 
		//		throw new FileNotFoundException($"Save was not found with provided path {SavePath + localPath}");

		//	BinaryReader reader = new(File.OpenRead(localPath));
		//	string fileVersion = reader.ReadString();
		//	if (!string.IsNullOrEmpty(version))
		//	{
		//		if (fileVersion != version) return fileVersion;
		//	}

		//	}
	}

	public static class SaveExtensions
	{
		public static void WriteCollection<T>(this BinaryWriter writer, ICollection<T> collection) where T : ISaveable
			=> WriteCollection(writer, collection, (w, o) => o.SaveFile(w));
		public static void WriteCollection<T>(this BinaryWriter writer, ICollection<T> collection, Action<BinaryWriter, T> saver)
		{
			if (writer == null) throw new NullReferenceException("Writer is null");
			if (collection == null) return;

			writer.Write(collection.Count);
			foreach (T item in collection)
			{
				saver(writer, item);
			}
		}
		public static IEnumerable<T> ReadCollection<T>(this BinaryReader reader) where T : ISaveable, new()
			=> ReadCollection(reader, r => { T v = new(); v.LoadFile(r); return v; });
		public static IEnumerable<T> ReadCollection<T>(this BinaryReader reader, Func<BinaryReader, T> loader)
		{
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				yield return loader(reader);
			}
		}
	}
}
