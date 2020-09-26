using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;


public class VersionEditor : EditorWindow
{
    [UsedImplicitly]
    private class PackageFormat
    {
        // ReSharper disable once InconsistentNaming
#pragma warning disable 649
        public string version;
#pragma warning restore 649
    }

    private int _major;
    private int _minor;
    private int _patch;

    [MenuItem("Gamesture/Build/Set Version", false, 10000)]
    public static void SetVersion()
    {
        VersionEditor window =
            (VersionEditor) GetWindow(typeof(VersionEditor), false, "Game Version");
        window.SetDefaultVersion();
        window.Show();
    }

    private static string GetCurrentVersion()
    {
        string versionScriptPath = Path.Combine(Application.dataPath, "Gamesture.CodeDocs", "package.json");
        string content = File.ReadAllText(versionScriptPath);
        return JsonUtility.FromJson<PackageFormat>(content).version;
    }

    private void SetDefaultVersion()
    {
        string[] versionParts = GetCurrentVersion().Split('.');
        _major = int.Parse(versionParts[0]);
        _minor = int.Parse(versionParts[1]);
        _patch = int.Parse(versionParts[2]);
    }

    public void OnGUI()
    {
        _major = EditorGUILayout.IntField("major: ", _major);
        _minor = EditorGUILayout.IntField("minor: ", _minor);
        _patch = EditorGUILayout.IntField("patch: ", _patch);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"VERSION: {_major}.{_minor}.{_patch}");
        EditorGUILayout.Space();

        if (GUILayout.Button("Set"))
        {
            UpdateVersion();
        }
    }

    private void UpdateVersion()
    {
        string version = $"{_major}.{_minor}.{_patch}";
        UpdateVersionFile(version);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void UpdateVersionFile(string version)
    {
        string versionScriptPath = Path.Combine(Application.dataPath, "Gamesture.CodeDocs", "package.json");
        string content = File.ReadAllText(versionScriptPath);
        PackageFormat packageFormat = JsonUtility.FromJson<PackageFormat>(content);
        content = content.Replace(packageFormat.version, $"{version}");
        File.WriteAllText(versionScriptPath, content);
    }
}
