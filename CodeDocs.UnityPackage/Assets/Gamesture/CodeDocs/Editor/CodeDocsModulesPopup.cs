using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    public class CodeDocsModulesPopup : EditorWindow
    {
        private const string MODULE_PREFIX = "/** \\addtogroup";
        private const string MODULE_POSTFIX = "*/";

        private List<Module> _modules;
        
        public static void Init()
        {
            CodeDocsModulesPopup window =
                (CodeDocsModulesPopup) GetWindow(typeof(CodeDocsModulesPopup), true, "CodeDocs Modules");
            window.Show();
        }

        [Serializable]
        private class Module
        {
            public string Name;
            public string Description;
        }

        private void ReadFile()
        {
            if (_modules != null || File.Exists(CodeDocsSettings.DoxygenFilePath) == false)
            {
                return;
            }
            
            _modules = new List<Module>();
            foreach (string line in File.ReadLines(CodeDocsSettings.DoxygenFilePath))
            {
                string output = line;
                if (output.StartsWith(MODULE_PREFIX))
                {
                    output = output.Replace(MODULE_PREFIX, "");
                    output = output.Replace(MODULE_POSTFIX, "");
                    output = output.Trim();

                    int spaceIndex = output.IndexOf(' ');
                    _modules.Add(new Module
                    {
                        Name = output.Substring(0, spaceIndex).Trim(),
                        Description = output.Substring(spaceIndex, output.Length - spaceIndex).Trim()
                    });
                }
            }
        }

        private void SaveFile()
        {
            if (_modules == null)
            {
                return;
            }
            
            StringBuilder content = new StringBuilder($"// generated automatically with {nameof(CodeDocsModulesPopup)}\n\n");
            
            foreach (Module module in _modules)
            {
                if (module == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(module.Name))
                {
                    Debug.LogError("cannot save doxygen file, module name is empty");
                    return;
                }

                if (module.Name.Trim().Any(Char.IsWhiteSpace))
                {
                    Debug.LogError("cannot save doxygen file, module name contains white space");
                    return;
                }
                
                content.AppendLine($"{MODULE_PREFIX} {module.Name} {module.Description} {MODULE_POSTFIX}\n");
            }
            
            if (File.Exists(CodeDocsSettings.DoxygenFilePath))
            {
                File.Delete(CodeDocsSettings.DoxygenFilePath);
            }
            
            File.WriteAllText(CodeDocsSettings.DoxygenFilePath, content.ToString());
        }

        private void OnGUI()
        {
            if (File.Exists(CodeDocsSettings.DoxygenFilePath) == false)
            {
                if (GUILayout.Button("CREATE FILE"))
                {
                    File.Create(CodeDocsSettings.DoxygenFilePath);
                }

                return;
            }

            ReadFile();
            
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("size", _modules.Count));
            
            while (newCount < _modules.Count)
            {
                _modules.RemoveAt( _modules.Count - 1 );
            }

            while (newCount > _modules.Count)
            {
                _modules.Add(new Module());
            }

            EditorGUILayout.BeginVertical();
            for(int i = 0; i < _modules.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _modules[i].Name = EditorGUILayout.TextField(_modules[i].Name);
                _modules[i].Description = EditorGUILayout.TextField(_modules[i].Description);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("SAVE"))
            {
                SaveFile();
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}