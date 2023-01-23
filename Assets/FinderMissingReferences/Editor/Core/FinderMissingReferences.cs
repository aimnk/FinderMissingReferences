using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Base class for working with lost links
    /// </summary>
    public class FinderMissingReferences
    {
        /// <summary>
        /// Find all lost links
        /// </summary>
        /// <param name="obj"></param>
        protected virtual List<MissingReferencesSubData> FindMissingReferences(Object obj, out int countAllMissingReferences)
        {
            List<MissingReferencesSubData> missingReferencesSceneDatas = new List<MissingReferencesSubData>();

            countAllMissingReferences = 0;

            var gameObject = obj as GameObject;;

            if (gameObject != null)
            {
                missingReferencesSceneDatas = FindMissingReferencesGameObject(gameObject, out int countMissingReferences);

                if (countMissingReferences > 0)
                {
                    countAllMissingReferences += countMissingReferences;
                    return missingReferencesSceneDatas;
                }
            }

            countAllMissingReferences += FindMissingReferencesObject(obj);

            return missingReferencesSceneDatas;
        }

        /// <summary>
        /// Finding lost references in GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        protected virtual List<MissingReferencesSubData> FindMissingReferencesGameObject(GameObject gameObject, out int countAllMissingReferences)
        {
            countAllMissingReferences = 0;

            List<MissingReferencesSubData> missingReferencesSceneDatas = new List<MissingReferencesSubData>();

            foreach (var component in gameObject.GetComponents<Component>())
            {
                int countMissing = FindMissingReferencesObject(component);

                if (countMissing > 0)
                {
                    countAllMissingReferences += countMissing;

                    missingReferencesSceneDatas.Add(
                        new MissingReferencesSubData(gameObject.name,
                            GlobalObjectId.GetGlobalObjectIdSlow(gameObject).ToString(),
                            countMissing));
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                missingReferencesSceneDatas.AddRange(FindMissingReferencesGameObject(child.gameObject,
                    out int countMissing));

                countAllMissingReferences += countMissing;
            }

            return missingReferencesSceneDatas;
        }
        
        /// <summary>
        /// Find all lost references in UnityEngine.Object
        /// </summary>
        /// <param name="gameObject"></param>
        protected virtual int FindMissingReferencesObject(Object obj)
        {
            var countMissingReferences = 0;

            if (obj == null)
            {
                countMissingReferences++;
                return countMissingReferences;
            }
            
            using (var serializedObject = new SerializedObject(obj))
            {
                var serializedProperty = serializedObject.GetIterator();

                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    if (serializedProperty.objectReferenceValue == null &&
                        serializedProperty.objectReferenceInstanceIDValue != 0)
                    {
                        countMissingReferences++;
                    }
                }
            }

            return countMissingReferences;
        }

        /// <summary>
        /// Search for lost links in the scene
        /// </summary>
        /// <param name="scene"></param>
        protected virtual List<MissingReferencesSubData> FindMissingReferencesInScene(Scene scene,
            out int countAllMissingReference)
        {
            countAllMissingReference = 0;

            List<MissingReferencesSubData> missingReferencesSceneDatas = new List<MissingReferencesSubData>();
            
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                missingReferencesSceneDatas.AddRange(FindMissingReferencesGameObject(rootObject,
                    out int countMissingReference));

                countAllMissingReference += countMissingReference;
            }

            return missingReferencesSceneDatas;
        }
    }   
}