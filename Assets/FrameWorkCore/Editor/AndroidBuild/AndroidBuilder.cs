using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;

namespace UnityGameFramework.Editor
{
    public class AndroidBuilder : MonoBehaviour
    {
        private const string outPath = ".build";

        [MenuItem("Builder/BuildAndroidTestApk")]
        public static void BuildTestApk()
        {
            RefreshAppVersion();
            CheckBuildDirectory();
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
            if (!defines.Contains("TEST"))
            {
                defines += ";TEST";
            }
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, defines);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[] { "Assets/Scenes/AppLauncher.unity" };
            //buildPlayerOptions.scenes = new string[] { "Assets/GleyPlugins/EasyIAP/Example/TestIAP.unity" };
            buildPlayerOptions.locationPathName = Path.Combine(outPath, "Temp.apk");
            buildPlayerOptions.assetBundleManifestPath = Path.Combine(Application.streamingAssetsPath, "ab", "StreamingAssets.manifest");
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Builder/BuildAndroidReleaseApk")]
        public static void BuildReleaseApk()
        {
            RefreshAppVersion();
            CheckBuildDirectory();
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
            if (defines.Contains("TEST"))
            {
                defines = defines.Replace(";TEST", string.Empty).Replace("TEST", string.Empty);
            }
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, defines);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[] { "Assets/Scenes/AppLauncher.unity" };
            buildPlayerOptions.locationPathName = Path.Combine(outPath, "Temp.apk");
            buildPlayerOptions.assetBundleManifestPath = Path.Combine(Application.streamingAssetsPath, "ab", "StreamingAssets.manifest");
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Builder/BuildAndroidReleaseAab")]
        public static void BuildReleaseAab()
        {
            RefreshAppVersion();
            CheckBuildDirectory();
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Public;
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
            if (defines.Contains("TEST"))
            {
                defines = defines.Replace(";TEST", string.Empty).Replace("TEST", string.Empty);
            }
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, defines);
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[] { "Assets/Scenes/AppLauncher.unity" };
            buildPlayerOptions.locationPathName = Path.Combine(outPath, "Temp.aab");
            buildPlayerOptions.assetBundleManifestPath = Path.Combine(Application.streamingAssetsPath, "ab", "StreamingAssets.manifest");
            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        private static void CheckBuildDirectory()
        {
            if (Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
        }

        public static void RefreshAppVersion()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                string[] args = System.Environment.GetCommandLineArgs();
                foreach (var s in args)
                {
                    if (s.Contains("--project_version"))
                    {
                        int version = int.Parse(s.Split(':')[1]);
                        PlayerSettings.bundleVersion = $"{version / 100}.{(version % 100) / 10}.{version % 10}";
                        Debug.Log($"PlayerSettings.bundleVersion:{PlayerSettings.bundleVersion}");
                        PlayerSettings.Android.bundleVersionCode = version;
                    }
                }
            }
        }
    }
}