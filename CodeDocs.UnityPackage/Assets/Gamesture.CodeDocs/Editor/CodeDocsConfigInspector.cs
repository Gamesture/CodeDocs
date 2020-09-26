using UnityEditor;

namespace Gamesture.CodeDocs
{
    [CustomEditor(typeof(CodeDocsConfig))]
    public class CodeDocsConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Please use menu: Gamesture/Code Docs/Settings", MessageType.Info);
        }
    }
}