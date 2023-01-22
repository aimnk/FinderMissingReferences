using System;
using System.Collections.Generic;
using UnityEditor;

namespace FinderMissingReferences.Editor.Core
{
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
        public List<MissingReferencesSceneData> MissingReferencesSceneData;
        
        public MissingReferencesData(string name, string pathToAsset, string guid, int countMissingGuid, 
            List<MissingReferencesSceneData> missingReferencesSceneData = null)
        {
            Name = name;
            PathToAsset = pathToAsset;
            GUID = guid;
            CountMissingGUID = countMissingGuid;
            MissingReferencesSceneData = missingReferencesSceneData;
        }
    }
    
    /// <summary>
    /// Дата потерянных ссылок внутри сцены
    /// </summary>
    [Serializable]
    public class MissingReferencesSceneData
    {
        public string Name;
        
        public GlobalObjectId LocalFileIdentifier { get; }
        
        /// <summary>
        /// The number of lost links for an asset
        /// </summary>
        public int CountMissingGUID { get; }
        
        public MissingReferencesSceneData(string name, GlobalObjectId localFileIdentifier, int countMissingGuid)
        {
            LocalFileIdentifier = localFileIdentifier;
            CountMissingGUID = countMissingGuid;
            Name = name;
        }
    }
}