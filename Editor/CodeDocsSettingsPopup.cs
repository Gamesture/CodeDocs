using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    public class CodeDocsSettingsPopup : EditorWindow
    {
        internal static void Init()
        {
            CodeDocsSettingsPopup window =
                (CodeDocsSettingsPopup) GetWindow(typeof(CodeDocsSettingsPopup), true, "CodeDocs Setup");
            window.minSize = new Vector2(600, 150);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(CodeDocsSettings.IsSetFromCode);
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            
            // docs root path
            CodeDocsSettings.DocsRootPath = EditorGUILayout.TextField("DOCS ROOT:", CodeDocsSettings.DocsRootPath);
            
            // docs root validation
            Color currentColor = GUI.contentColor;
            bool isValid = CodeDocsSettings.IsProperRootPath;
            GUI.contentColor = isValid ? Color.green : Color.red;
            EditorGUILayout.LabelField(isValid ? "valid" : "INVALID", GUILayout.Width(50));
            GUI.contentColor = currentColor;
            
            EditorGUILayout.EndHorizontal();
            
            // docs root selection
            if (GUILayout.Button("select", GUILayout.Width(50)))
            {
                CodeDocsSettings.DocsRootPath =
                    EditorUtility.OpenFolderPanel("Docs Root Path", Path.Combine(Application.dataPath, ".."), "");
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            
            // sources path
            CodeDocsSettings.SourcesPath = EditorGUILayout.TextField("SOURCES:", CodeDocsSettings.SourcesPath);
            
            // sources validation
            currentColor = GUI.contentColor;
            isValid = CodeDocsSettings.IsProperSourcesPath;
            GUI.contentColor = isValid ? Color.green : Color.red;
            EditorGUILayout.LabelField(isValid ? "valid" : "INVALID", GUILayout.Width(50));
            GUI.contentColor = currentColor;
            
            EditorGUILayout.EndHorizontal();
            
            // sources selection
            if (GUILayout.Button("select", GUILayout.Width(50)))
            {
                CodeDocsSettings.SourcesPath =
                    EditorUtility.OpenFolderPanel("Sources Path", Path.Combine(Application.dataPath, ".."), "");
            }
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space();

            if (CodeDocsSettings.IsSetFromCode)
            {
                GUILayout.Label($"Cannot modify settings, it was set from code, find method {nameof(CodeDocsSettings.SetConfiguration)}");
                if (GUILayout.Button("CLEAR ALL", GUILayout.Width(150)))
                {
                    CodeDocsSettings.ClearConfig();
                }
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}