using System;
using System.Collections.Generic;

namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Search for lost links across all assets
    /// </summary>
    public class FinderMissingReferencesEverywhere: IMissingFinder
    {
        public List<MissingReferencesData> FindMissingReferences(Action<int, int> onProgressSearch = null)
        {
            IMissingFinder missingFinderAllScenes = new FInderMissingReferencesAllScenes();
            IMissingFinder missingFinderInAssets = new FinderMissingReferencesInAssets();
            
            List<MissingReferencesData> missingReferencesDatas =
                missingFinderAllScenes.FindMissingReferences(onProgressSearch);
            
            missingReferencesDatas.AddRange(missingFinderInAssets.FindMissingReferences(onProgressSearch));
            
            return missingReferencesDatas;
        }
    }
}