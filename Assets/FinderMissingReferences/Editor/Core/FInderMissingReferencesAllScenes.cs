using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Search for lost links in all scenes
    /// </summary>
    public class FInderMissingReferencesAllScenes: FinderMissingReferences, IMissingFinder
    {
        private readonly string[] _extensionFiles = 
        {
            ".unity"
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
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(paths[i]);

                if (sceneAsset == null)
                {
                    continue;
                }

                var scene = EditorSceneManager.OpenScene(paths[i]);

                List<MissingReferencesSubData> missingReferencesSubDatas = FindMissingReferencesInScene(scene,
                    out int countMissingReferences);

                if (countMissingReferences <= 0)
                {
                    continue;
                }

                missingReferencesDatas.Add(new MissingReferencesData(scene.name, paths[i],
                    AssetDatabase.AssetPathToGUID(paths[i]), countMissingReferences, missingReferencesSubDatas));
            }

            onProgressSearch?.Invoke(paths.Count, paths.Count);
            return missingReferencesDatas;
        }
    }
}