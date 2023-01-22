namespace FinderMissingReferences.Editor.Core
{
    /// <summary>
    /// Type of search for lost links
    /// </summary>
    public enum SearchType
    {
        ReferencesInCurrentScene = 0,
        ReferencesInBuild = 1,
        AllReferences = 2,
        AllScenes = 3,
        Everywhere = 4
    }
}