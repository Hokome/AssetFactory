using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Language
{
	public Dictionary<string, string> pairs;
	public string id = "en";
	public string name = "English";
	public string enName = "English";


	public static readonly string path = $@"{Application.dataPath}/Localization";
	public static Language current;

	public static Language FromJson(string json)
	{
		Language l = JsonConvert.DeserializeObject<Language>(json);
		return l;
	}
	public static void SelectLanguage(string id)
	{
		string p = $@"{path}/lang-{id}.json";
		string s = System.IO.File.ReadAllText(p);
		current = FromJson(s);
	}

	public static string CodeToName(string code)
	{
		switch (code)
		{
			case "en":
				return "English";
			case "fr":
				return "Français";
			case "jp":
				return "日本語";
			default:
				return "NULL";
		}
	}
	public static string NameToCode(string name)
	{
		switch (name)
		{
			case "English":
				return "en";
			case "Français":
				return "fr";
			case "日本語":
				return "jp";
			default:
				return "NULL";
		}
	}
}
