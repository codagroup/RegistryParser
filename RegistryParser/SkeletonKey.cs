namespace CODA.RegistryParser;
public class SkeletonKey
{
    #region Constructors
    public SkeletonKey(string keyPath, string keyName, bool addValues)
    {
        KeyPath = keyPath;
        KeyName = keyName;
        AddValues = addValues;
        Subkeys = new List<SkeletonKey>();
    }
    #endregion
    #region Properties
    public string KeyName { get; }
    public string KeyPath { get; }
    public bool AddValues { get; }
    public List<SkeletonKey> Subkeys { get; }
    #endregion
}