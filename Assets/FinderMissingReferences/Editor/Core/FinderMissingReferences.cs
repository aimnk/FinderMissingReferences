using System.Collections.Generic;
using Unity.VisualScripting;
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
        protected virtual int FindMissingReferences(Object obj)
        {
            var gameObject = obj.GameObject();

            int countAllMissingReferences = 0;
            
            if (gameObject != null)
            {
                return FindMissingReferencesGameObject(gameObject);
            }

            countAllMissingReferences += FindMissingReferencesObject(obj);
            
            return countAllMissingReferences;
        }

        /// <summary>
        /// Finding lost references in GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        protected virtual int FindMissingReferencesGameObject(GameObject gameObject)
        {
            var countAllMissingReferences = 0;
            
            foreach (var component in gameObject.GetComponents<Component>())
            {
                countAllMissingReferences += FindMissingReferencesObject(component);
            }
            
            return countAllMissingReferences;
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
        protected virtual List<MissingReferencesSceneData> FindMissingReferencesInScene(Scene scene,
            out int countAllMissingReference)
        {
            countAllMissingReference = 0;

            List<MissingReferencesSceneData> missingReferencesSceneDatas = new List<MissingReferencesSceneData>();
            
            var queue = new Queue<MissingReferencesSceneData>();
            
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                int countMissingReference = FindMissingReferencesGameObject(rootObject);
                
                if (countMissingReference > 0)
                {
                    missingReferencesSceneDatas.Add(
                        new MissingReferencesSceneData(rootObject.name,
                            GlobalObjectId.GetGlobalObjectIdSlow(rootObject),
                            countMissingReference));
                }

                countAllMissingReference += countMissingReference;
            }
            
            return missingReferencesSceneDatas;
        }
    }   
}