using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreatePackage
{
    private static string DocsRootDestinationPath =>
        Path.Combine(Application.dataPath, "Gamesture.CodeDocs", "Editor", "CodeDocs");
    
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
        
        CopyDocsRoot();
        AssetDatabase.Refresh();

        AssetDatabase.ExportPackage(SOURCE, destination,
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        RemoveDocsRoot();
        AssetDatabase.Refresh();
    }

    private static void RemoveDocsRoot()
    {
        Directory.Delete(DocsRootDestinationPath, true);
        File.Delete($"{DocsRootDestinationPath}.meta");
    }

    private static void CopyDocsRoot()
    {
        string sourcePath = Path.Combine(Application.dataPath, "..", "CodeDocs");
        string destinationPath = DocsRootDestinationPath;
        
        // create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
        }

        // copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }
    }
}
