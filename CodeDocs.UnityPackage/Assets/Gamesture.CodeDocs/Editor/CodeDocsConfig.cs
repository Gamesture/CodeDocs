using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CodeDocsConfig : ScriptableObject
    {
        public string DocsRootPath;
        public string SourcesPath;
        public bool IsSetFromCode;
    }
}