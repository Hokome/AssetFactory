using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AssetFactory
{
    public class CSVReader
    {
		public readonly string path;

		private readonly List<string[]> values;
		private readonly Dictionary<string, int> headers;

		public readonly int width;
		public readonly int height;

		public CSVReader(string path)
		{
			this.path = path;

			int count = File.ReadLines(path).Count();
			var reader = new StreamReader(path, System.Text.Encoding.UTF8);
			values = new List<string[]>(count);
			while (!reader.EndOfStream)
			{
				string[] vs = Split(reader.ReadLine());
				width = Mathf.Max(width, vs.Length);
				values.Add(vs);
				height++;
			}
			headers = new Dictionary<string, int>(values[0].Length);
			for (int i = 0; i < values[0].Length; i++)
				headers.Add(values[0][i], i);
		}

		public string ReadString(string header, int row) => ReadString(headers[header], row);
		public string ReadString(int column, int row) => values[row][column];

		public int ReadInt(int column, int row) => int.Parse(ReadString(column, row));
		public int ReadInt(string header, int row) => int.Parse(ReadString(header, row));

		public float ReadFloat(int column, int row) => float.Parse(ReadString(column, row));
		public float ReadFloat(string header, int row) => float.Parse(ReadString(header, row));

		private static string[] Split(string file)
		{

			string[] list = file.Split(',');
			for (int i = 0; i < list.Length; i++)
			{
				list[i] = list[i].Replace("&c", ",");
			}
			return list;
		}
	}
}
