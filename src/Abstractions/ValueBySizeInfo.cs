namespace CODA.RegistryParser.Abstractions;
public class ValueBySizeInfo
{
    #region Constructors
    public ValueBySizeInfo(RegistryKey key, KeyValue value)
    {
        Key = key;
        Value = value;
    }
    #endregion
    #region Properties
    public RegistryKey Key {get;}
    public KeyValue Value {get;}
    #endregion
}