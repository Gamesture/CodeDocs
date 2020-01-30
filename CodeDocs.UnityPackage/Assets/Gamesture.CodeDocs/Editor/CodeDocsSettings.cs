using System.IO;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    public static class CodeDocsSettings
    {
        private const string DOCS_ROOT_PATH_KEY = "gamesture.codedocs.doxygen_path";
        private const string SOURCES_PATH_KEY = "gamesture.codedocs.sources_path";
        private const string SET_WITH_CODE = "gamesture.codedocs.set_from_code";

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
            return File.Exists(Path.Combine(DocsRootPath, "GeneratedDocs", "html", "index.html"));
        }

        public static string DocsRootPath
        {
            get => UnityEditor.EditorPrefs.GetString(DOCS_ROOT_PATH_KEY);
            set => UnityEditor.EditorPrefs.SetString(DOCS_ROOT_PATH_KEY, value);
        }

        public static string SourcesPath
        {
            get => UnityEditor.EditorPrefs.GetString(SOURCES_PATH_KEY);
            set => UnityEditor.EditorPrefs.SetString(SOURCES_PATH_KEY, value);
        }

        public static bool IsSetFromCode
        {
            get => UnityEditor.EditorPrefs.GetBool(SET_WITH_CODE);
            private set => UnityEditor.EditorPrefs.SetBool(SET_WITH_CODE, value);
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
            UnityEditor.EditorPrefs.DeleteKey(DOCS_ROOT_PATH_KEY);
            UnityEditor.EditorPrefs.DeleteKey(SOURCES_PATH_KEY);
            UnityEditor.EditorPrefs.DeleteKey(SET_WITH_CODE);
        }
    }
}