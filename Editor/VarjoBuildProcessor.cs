using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using UnityEngine;

using Varjo.XR;

namespace Varjo.XR.Editor
{
    public class VarjoBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0;  }
        }

        void CleanOldSettings()
        {
            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null)
                return;

            var oldSettings = from s in preloadedAssets
                where s != null && s.GetType() == typeof(VarjoSettings)
                select s;

            if (oldSettings != null && oldSettings.Any())
            {
                var assets = preloadedAssets.ToList();
                foreach (var s in oldSettings)
                {
                    assets.Remove(s);
                }

                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        private static void CreateDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
                CreateDirectory(directory.Parent);

            if (!directory.Exists)
                directory.Create();
        }

        /// <summary>Override of <see cref="IPreprocessBuildWithReport"></summary>
        /// <param name="report">Build report.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            CleanOldSettings();

            VarjoSettings settings = null;
            EditorBuildSettings.TryGetConfigObject("Varjo.XR.Settings", out settings);
            if (settings == null)
                return;

            UnityEngine.Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(settings))
            {
                var assets = preloadedAssets.ToList();
                assets.Add(settings);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPostprocessBuildWithReport"></summary>
        /// <param name="report">Build report.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            CleanOldSettings();

            if (report.summary.platform != BuildTarget.StandaloneWindows64) return;

            // Copy the input config files into the StreamingAssets in the build directory after building the project.
            FileInfo buildPath = new FileInfo(report.summary.outputPath);
            string buildName = buildPath.Name.Replace(buildPath.Extension, "");
            DirectoryInfo buildDirectory = buildPath.Directory;

            string dataDirectory = Path.Combine(buildDirectory.FullName, buildName + "_Data");
            if (!Directory.Exists(dataDirectory))
            {
                Debug.LogError($"Could not find data directory at: {dataDirectory}.");
            }

            string configsPath = Path.Combine(new string[] { dataDirectory, "StreamingAssets", "Varjo", "Input", "Configs" });
            CreateDirectory(new DirectoryInfo(configsPath));

            string sourceDir = Path.GetFullPath("Packages/com.varjo.xr/Runtime/Input/Configs");
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                if (Path.GetExtension(file) != ".meta")
                {
                    File.Copy(file, Path.Combine(configsPath, Path.GetFileName(file)), true);
                }
            }
        }
    }
}
