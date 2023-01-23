using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Search for lost links in all assets (without scenes)
    /// </summary>
    public class FinderMissingReferencesInAssets : FinderMissingReferences, IMissingFinder
    {
        private readonly string[] _extensionFiles = 
        {
            ".prefab",
            ".asset"
        };
        
        List<MissingReferencesData> IMissingFinder.FindMissingReferences(Action<int, int> onProgressSearch)
        {
            List<MissingReferencesData> missingReferencesDatas = new List<MissingReferencesData>();

            var paths = AssetDatabase.GetAllAssetPaths().ToList();
            paths.RemoveAll(string.IsNullOrWhiteSpace);
            paths = paths.Where(path => _extensionFiles.Any(path.EndsWith)).ToList();

            for (var i = 0; i < paths.Count; i++)
            {
                onProgressSearch?.Invoke(i, paths.Count);

                var obj = AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));

                if (obj == null)
                {
                    continue;
                }

                List<MissingReferencesSubData> missingReferencesSceneDatas =
                    FindMissingReferences(obj, out int countAllMissingReferences);

                if (countAllMissingReferences <= 0)
                {
                    continue;
                }

                var missingReferencesData = new MissingReferencesData(
                    obj.name,
                    AssetDatabase.GetAssetPath(obj),
                    AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(obj)).ToString(),
                    countAllMissingReferences, missingReferencesSceneDatas);

                missingReferencesDatas.Add(missingReferencesData);
            }

            onProgressSearch?.Invoke(paths.Count, paths.Count);

            return missingReferencesDatas;
        }
    }
}
