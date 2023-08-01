using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

//Originally from AssetFactory
namespace AssetFactory
{
	public class DebugEx : MonoBehaviour
	{
		public static DebugEx Inst { get; private set; }
#if !UNITY_SERVER
		#region GUI debug
		static KeyControl[] keys;

		public static bool Activated { get; private set; } = false;
		public static Vector2 position = new Vector2(10f, 10f);
		public static float width = 200f;
		static int MaxRenderLines { get => MaxLines + 1; }
		public static int MaxLines { get; set; } = 3;

		const float LINE_HEIGHT = 15f;

		static object[] elements = new object[MaxLines];

		public static void SetElement(int index, object o)
		{
			if (index >= elements.Length)
				return;
			elements[index] = o;
		}
		private void Awake()
		{
			if (Inst == null)
				Inst = this;
			else
			{
				Destroy(gameObject);
				return;
			}
			DontDestroyOnLoad(gameObject);
			keys = new KeyControl[2]
			{
			Keyboard.current.leftShiftKey,
			Keyboard.current.f3Key
			};
		}
		void Update()
		{
			if (KeysPressed())
				Activated = !Activated;
		}
		bool KeysPressed()
		{
			if (keys == null || keys.Length < 1)
				return false;
			for (int i = 0; i < keys.Length - 1; i++)
			{
				if (keys[i].isPressed)
					continue;
				return false;
			}
			return keys[keys.Length - 1].wasPressedThisFrame;
		}
		void OnGUI()
		{
			if (!Activated)
				return;
			string s = "Debug";
			foreach (var item in elements)
			{
				if (item == null)
				{
					s += "\n-";
					continue;
				}
				s += '\n' + item.ToString();
			}
			GUI.Box(new Rect(position, new Vector2(width, MaxRenderLines * LINE_HEIGHT + 10f)), s);
		}
		#endregion

#endif
		#region Gizmos
		public static void DrawRect(Rect r, Color color)
		{
			Vector2 sw = r.position;
			Vector2 ne = r.position + r.size;
			Vector2 se = new Vector2(r.x + r.width, r.y);
			Vector2 nw = new Vector2(r.x, r.y + r.height);

			Gizmos.color = color;
			Gizmos.DrawLine(nw, ne);
			Gizmos.DrawLine(ne, se);
			Gizmos.DrawLine(se, sw);
			Gizmos.DrawLine(sw, nw);
		}
		public static void DrawCross(Vector3 point, Color color, float size = 1f)
		{
			Debug.DrawLine(point + Vector3.up * size, point + Vector3.down * size, color);
			Debug.DrawLine(point + Vector3.right * size, point + Vector3.left * size, color);
			Debug.DrawLine(point + Vector3.forward * size, point + Vector3.back * size, color);
		}
		#endregion

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void StartUp()
		{
#if !UNITY_SERVER
			if (Inst == null)
				Inst = new GameObject("Debugger").AddComponent<DebugEx>();
#endif
			string path = $@"{Application.dataPath}/debug.log";
			logStream = new FileStream(path, FileMode.OpenOrCreate);
			logWriter = new StreamWriter(logStream);

			logWriter.WriteLine();
			logWriter.WriteLine("------------ New Session -------------");
			logWriter.WriteLine();
		}

		private static FileStream logStream;
		private static StreamWriter logWriter;
		public static void LogFile(object message)
		{
			logWriter.WriteLine(message.ToString());
		}
		private void OnApplicationQuit()
		{
			logWriter.Close();
		}
	}
}