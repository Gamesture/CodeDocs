using System.IO;
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
            ReadConfigFile();

            if (IsProperRootPath)
            {
                return;
            }
            
            string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "..", "Library", "PackageCache"),
                "doxygen.exe", SearchOption.AllDirectories);

            if (files.Length != 1)
            {
                Debug.LogError("cannot find doxygen folder");
                return;
            }
            
            DocsRootPath = Path.GetDirectoryName(Path.GetFullPath(files[0]));
        }

        public static bool IsProperlyConfigured()
        {
            return IsProperRootPath && IsProperSourcesPath;
        }

        public static bool IsProperRootPath => File.Exists(GenerateDocsExe);
        
        public static bool IsProperSourcesPath => Directory.Exists(SourcesPath);

        public static string DoxygenFilePath => Path.Combine(SourcesPath, "doxygen.txt");

        public static bool AreDocsGenerated()
        {
            return File.Exists(Path.Combine(DocsRootPath, "GeneratedDocs~", "html", "index.html"));
        }

        public static string DocsRootPath
        {
            get => _config.DocsRootPath;
            set => _config.DocsRootPath = value;
        }

        public static string SourcesPath
        {
            get => _config.SourcesPath;
            set => _config.SourcesPath = value;
        }

        public static bool IsSetFromCode
        {
            get => _config.IsSetFromCode;
            private set => _config.IsSetFromCode = value;
        }

        public static string OpenDocsExe => Path.Combine(DocsRootPath, $"open.{GetExeExtension()}");
        public static string GenerateDocsExe => Path.Combine(DocsRootPath, $"generate.{GetExeExtension()}");
        
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

        public static void ShowConfiguration()
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

        public static void ClearConfig()
        {
            _config.SourcesPath = null;
            _config.DocsRootPath = null;
            _config.IsSetFromCode = false;
        }

        private static void ReadConfigFile()
        {
            if (_config != null)
            {
                return;
            }

            const string CONFIG_PATH = "Gamesture/CodeDocs/Config.asset";
            _config = AssetDatabase.LoadAssetAtPath<CodeDocsConfig>($"Assets/{CONFIG_PATH}");
            if (_config != null)
            {
                return;
            }

            string dir = Path.Combine(Application.dataPath, Path.GetDirectoryName(CONFIG_PATH));
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            CodeDocsConfig configObject = ScriptableObject.CreateInstance<CodeDocsConfig>();
            AssetDatabase.CreateAsset(configObject, $"Assets/{CONFIG_PATH}");
            _config = Resources.Load<CodeDocsConfig>($"Assets/{CONFIG_PATH}");
        }
    }
}