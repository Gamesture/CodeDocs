using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    public static class CodeDocsSettings
    {
        private static CodeDocsConfig _config;

        static CodeDocsSettings()
        {
            AutoSetup();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AutoSetup()
        {
            if (IsProperRootPath)
            {
                return;
            }
            
            string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "..", "Library", "PackageCache"),
                "doxygen.exe", SearchOption.AllDirectories);

            if (files.Length == 1)
            {
                DocsRootPath = Path.GetDirectoryName(Path.GetFullPath(files[0]));
            }
        }

        internal static bool IsProperlyConfigured()
        {
            return IsProperRootPath && IsProperSourcesPath;
        }

        internal static bool IsProperRootPath => File.Exists(GenerateDocsExe);

        internal static bool IsProperSourcesPath => Directory.Exists(SourcesPath);

        internal static string DoxygenFilePath => Path.Combine(SourcesPath, "doxygen.txt");

        internal static bool AreDocsGenerated()
        {
            return File.Exists(Path.Combine(DocsRootPath, "GeneratedDocs~", "html", "index.html"));
        }

        internal static string DocsRootPath
        {
            get => GetConfig()?.DocsRootPath;
            set
            {
                CodeDocsConfig config = GetConfig();
                if (config != null)
                {
                    config.DocsRootPath = value;
                }
            }
        }

        internal static string SourcesPath
        {
            get => GetConfig()?.SourcesPath;
            set
            {
                CodeDocsConfig config = GetConfig();
                if (config != null)
                {
                    config.SourcesPath = value;
                }
            }
        }

        internal static bool IsSetFromCode
        {
            get => GetConfig()?.IsSetFromCode ?? false;
            private set
            {
                CodeDocsConfig config = GetConfig();
                if (config != null)
                {
                    config.IsSetFromCode = value;
                }
            }
        }

        internal static string OpenDocsExe => Path.Combine(DocsRootPath, $"open.{GetExeExtension()}");
        internal static string GenerateDocsExe => Path.Combine(DocsRootPath, $"generate.{GetExeExtension()}");
        
        private static string GetExeExtension()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor: return "sh";
                case RuntimePlatform.WindowsEditor: return "bat";
                default:
                    Debug.LogError("unknown editor platform, don't know what to start");
                    return "";
            }
        }

        internal static void ShowConfiguration()
        {
            CodeDocsSettingsPopup.Init();
        }

        /// <summary>
        /// Use this method to setup configuration from code, for example on Unity editor startup.
        /// It will prevent new programmers to input paths manually.
        /// </summary>
        /// <param name="sourcesPath">Path to folder containing source files from which documentation will be generated</param>
        public static void SetSourcesPath(string sourcesPath)
        {
            IsSetFromCode = true;
            SourcesPath = sourcesPath;
        }

        internal static void ClearConfig()
        {
            CodeDocsConfig config = GetConfig();
            if (config == null)
            {
                return;
            }

            config.SourcesPath = null;
            config.DocsRootPath = null;
            config.IsSetFromCode = false;
        }

        [CanBeNull]
        private static CodeDocsConfig GetConfig()
        {
            if (_config != null)
            {
                return _config;
            }

            const string CONFIG_PATH = "Gamesture/CodeDocs/Config.asset";
            _config = AssetDatabase.LoadAssetAtPath<CodeDocsConfig>($"Assets/{CONFIG_PATH}");
            if (_config != null)
            {
                return _config;
            }

            string dirName = Path.GetDirectoryName(CONFIG_PATH);
            if (dirName == null)
            {
                return null;
            }

            string dir = Path.Combine(Application.dataPath, dirName);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            CodeDocsConfig configObject = ScriptableObject.CreateInstance<CodeDocsConfig>();
            AssetDatabase.CreateAsset(configObject, $"Assets/{CONFIG_PATH}");
            _config = Resources.Load<CodeDocsConfig>($"Assets/{CONFIG_PATH}");
            return _config;
        }
    }
}