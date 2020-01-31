### Include CodeDocs in Unity project:
- add to file Packages/manifest.json following line:\
`"com.gamesture.code_docs": "https://github.com/Gamesture/CodeDocs.git#__commit_hash__",`\
where __commit_hash__ is the latest commit hash from upm branch.

- setup source dir by choosing menu _Gamesture/Code Docs/Configure_\
(usual scripts location is `<your_unity_project>/Assets/Scripts`)

- (optional but recommended) automatically set sources path\
Create file in your project to automatically set sources path, i.e.\
```csharp
    public static class CodeDocsSettings
    {
        static CodeDocsSettings()
        {
            Setup();
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void Setup()
        {
            Gamesture.CodeDocs.CodeDocsSettings.SetSourcesPath(Path.Combine(Application.dataPath, "Scripts"));
        }
    }
```

### To include CodeDocs in non-Unity project:
- make this repository as submodule to your repository
- make batch file that executes generate.bat (Windows) or generate.sh (Mac) with first parameter is name of the product and second is root folder of your sources\
i.e. `generate.sh Questland ~/project/questland_client/Assets/Scripts`\
 (scripts location is `CodeDocs.UnityPackage/Assets/Gamesture.CodeDocs/Executable`)

### Creating new release
When you want to release new version of this package, just commit everyting to master branch and execute `create_release.sh` script (use Mac or console in Git Extensions on Windows).
After that notify everyone using this package to update it to the newest commit.