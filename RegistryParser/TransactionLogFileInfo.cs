namespace CODA.RegistryParser;

public class TransactionLogFileInfo
{
    #region Constructors
    public TransactionLogFileInfo(string fileName, byte[] fileBytes)
    {
        FileName = fileName;
        FileBytes = fileBytes;
    }
    #endregion
    #region Properties
    public string FileName { get; }
    public byte[] FileBytes { get; }
    #endregion
}