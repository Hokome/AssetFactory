#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Util
{
	public class AutoBuilder
	{
		static readonly string BUILD_SETTINGS_PATH = $@"Assets/AutoBuild/";
		static readonly string BUILD_PATH = $@"{Directory.GetParent(Application.dataPath)}/Builds/";

		//[MenuItem("Build/Build All")]
		public static void BuildAll()
		{
			BuildServer();
			BuildClient();
		}
		//[MenuItem("Build/Build Server")]
		public static void BuildServer()
		{
			AutoBuildSettings settings = AssetDatabase.LoadAssetAtPath<AutoBuildSettings>($@"{BUILD_SETTINGS_PATH}Server_Autobuild_Settings.asset");
			BuildPlayerOptions bpo = GetOptions(settings);
			bpo.locationPathName = @$"{BUILD_PATH}Server/{PlayerSettings.productName} Server.exe";

			BuildReport br = BuildPipeline.BuildPlayer(bpo);
			LogSummary(br);
		}
		[MenuItem("Build/Build Client")]
		public static void BuildClient()
		{
			AutoBuildSettings settings = AssetDatabase.LoadAssetAtPath<AutoBuildSettings>($@"{BUILD_SETTINGS_PATH}Client_Autobuild_Settings.asset");
			BuildPlayerOptions bpo = GetOptions(settings);
			bpo.locationPathName = @$"{BUILD_PATH}Client/{PlayerSettings.productName}.exe";

			BuildReport br = BuildPipeline.BuildPlayer(bpo);
			LogSummary(br);
		}
		public static void AddBuildScriptingDefines(string defineString)
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				BuildTargetGroup.Standalone, 
				PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone) 
				+ ';' + defineString);
		}
		static BuildPlayerOptions GetOptions(AutoBuildSettings settings)
		{
			return new BuildPlayerOptions()
			{
				scenes = settings.GetScenesPath(),
				target = BuildTarget.StandaloneWindows64
			};
		}

		static void LogSummary(BuildReport report)
		{
			if (report.summary.result == BuildResult.Failed)
			{
				Debug.LogError($"Build failed there were {report.summary.totalErrors} errors");
			}
			else if (report.summary.result == BuildResult.Succeeded)
			{
				Debug.Log($"Build succeeded in {report.summary.totalTime}. Build size is {report.summary.totalSize}");
			}
		}
	}
}
#endif