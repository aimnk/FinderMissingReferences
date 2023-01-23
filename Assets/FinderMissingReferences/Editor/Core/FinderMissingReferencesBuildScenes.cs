using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Search for lost links in scenes included in the build
    /// </summary>
    public class FinderMissingReferencesBuildScenes : FinderMissingReferences, IMissingFinder
    {
        List<MissingReferencesData> IMissingFinder.FindMissingReferences(Action<int ,int> onProgressSearch)
        {
            List<MissingReferencesData> missingReferencesDatas = new List<MissingReferencesData>();
            
            var editorBuildSettingsScenes = EditorBuildSettings.scenes
                .Where(scenes => scenes.enabled).ToList();
            
            for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
            {
                onProgressSearch?.Invoke(i, editorBuildSettingsScenes.Count);
                
                var scene = EditorSceneManager.OpenScene(editorBuildSettingsScenes[i].path);

                List<MissingReferencesSubData> missingReferencesSubDatas = FindMissingReferencesInScene(scene,
                    out int countMissingReferences);

                if (countMissingReferences <= 0)
                {
                    continue;
                }

                missingReferencesDatas.Add(new MissingReferencesData(scene.name, editorBuildSettingsScenes[i].path,
                    AssetDatabase.AssetPathToGUID(editorBuildSettingsScenes[i].path), countMissingReferences, missingReferencesSubDatas));
            }

            onProgressSearch?.Invoke(editorBuildSettingsScenes.Count, editorBuildSettingsScenes.Count);
            return missingReferencesDatas;
        }
    }
}
