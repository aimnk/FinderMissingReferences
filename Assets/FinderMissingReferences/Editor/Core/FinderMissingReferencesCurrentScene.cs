using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Search for lost links in the current scene
    /// </summary>
    public class FinderMissingReferencesCurrentScene : FinderMissingReferences, IMissingFinder
    {
        List<MissingReferencesData> IMissingFinder.FindMissingReferences(Action<int, int> onProgressSearch)
        {
            List<MissingReferencesData> missingReferencesDatas = new List<MissingReferencesData>();

            Scene scene = SceneManager.GetActiveScene();

            List<MissingReferencesSubData> missingReferencesSubDatas = FindMissingReferencesInScene(scene,
                out int countMissingReferences);

            if (countMissingReferences >= 0)
            {
                missingReferencesDatas.Add(new MissingReferencesData(scene.name, scene.path,
                    AssetDatabase.AssetPathToGUID(scene.path), countMissingReferences, missingReferencesSubDatas));
            }

            onProgressSearch?.Invoke(1, 1);
            return missingReferencesDatas;
        }
    }
}