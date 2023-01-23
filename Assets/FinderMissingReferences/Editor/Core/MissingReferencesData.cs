using System;
using System.Collections.Generic;
using UnityEditor;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Date of lost links
    /// </summary>
    [Serializable]
    public class MissingReferencesData
    {
        public string Name { get; }

        /// <summary>
        /// The path to the asset in the project
        /// </summary>
        public string PathToAsset { get; }

        public string GUID { get; }

        /// <summary>
        /// Is additional information shown in the editor window?
        /// </summary>
        public bool IsShow;
        
        /// <summary>
        /// Number of lost links for an asset
        /// </summary>
        public int CountMissingGUID { get; }

        /// <summary>
        /// Date of lost links inside the scene
        /// </summary>
        public List<MissingReferencesSubData> MissingReferencesSceneData;
        
        public MissingReferencesData(string name, string pathToAsset, string guid, int countMissingGuid, 
            List<MissingReferencesSubData> missingReferencesSceneData = null)
        {
            Name = name;
            PathToAsset = pathToAsset;
            GUID = guid;
            CountMissingGUID = countMissingGuid;
            MissingReferencesSceneData = missingReferencesSceneData;
        }
    }
    
    /// <summary>
    /// Date of lost links nested objects
    /// </summary>
    [Serializable]
    public class MissingReferencesSubData
    {
        public string Name;
        
        /// <summary>
        /// GlobalObjectId in string
        /// </summary>
        public string GlobalObjectId { get; }
        
        /// <summary>
        /// The number of lost links for an asset
        /// </summary>
        public int CountMissingGUID { get; }
        
        public MissingReferencesSubData(string name, string globalObjectId, int countMissingGuid)
        {
            GlobalObjectId = globalObjectId;
            CountMissingGUID = countMissingGuid;
            Name = name;
        }
    }
}