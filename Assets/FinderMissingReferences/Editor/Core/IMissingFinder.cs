using System;
using System.Collections.Generic;

namespace FinderMissingReferences.Editor.Core
{
    public interface IMissingFinder
    {
        /// <summary>
        /// Find lost links
        /// </summary>
        public List<MissingReferencesData> FindMissingReferences(Action<int, int> onProgressSearch = null);
    }
}