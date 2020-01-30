using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreatePackage
{
    [MenuItem("Gamesture/Export Package", false, 20)]
    public static void Export()
    {
        const string SOURCE = "Assets/Gamesture.CodeDocs";
        string destination = Path.Combine(Application.dataPath, "..", "build", "Gamesture.CodeDocs.unitypackage");
        string destinationDir = Path.GetDirectoryName(destination);

        if (destinationDir != null && Directory.Exists(destinationDir) == false)
        {
            Directory.CreateDirectory(destinationDir);
        }

        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        AssetDatabase.ExportPackage(SOURCE, destination,
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
    }
}
