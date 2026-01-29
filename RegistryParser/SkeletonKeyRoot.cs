namespace CODA.RegistryParser;
public class SkeletonKeyRoot
{
    #region Constructors
    public SkeletonKeyRoot(string keyPath, bool addValues, bool recursive)
    {
        KeyPath = keyPath;
        AddValues = addValues;
        Recursive = recursive;
    }
    #endregion
    #region Properties
    public string KeyPath { get; }
    public bool AddValues { get; }
    public bool Recursive { get; }
    #endregion
}