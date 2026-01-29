#region Usings
using System.Text;
using CODA.RegistryParser.Cells;
#endregion

namespace CODA.RegistryParser.Abstractions;
/// <summary>
///     Represents a key that is associated with a Registry hive
///     <remarks>Also contains references to low level structures related to a given key</remarks>
/// </summary>
public class RegistryKey
{
    #region Enums
    [Flags]
    public enum KeyFlagsEnum
    {
        Deleted = 1,
        HasActiveParent = 2
    }
    #endregion
    #region Fields
    private string _keyPath;
    #endregion
    #region Constructors
    public RegistryKey(NkCellRecord nk, RegistryKey? parent)
    {
        NkRecord = nk;

        Parent = parent;

        InternalGuid = Guid.NewGuid().ToString();

        SubKeys = new List<RegistryKey>();
        Values = new List<KeyValue>();

        ClassName = string.Empty;
        _keyPath = string.Empty;
    }
    #endregion
    #region Properties
    public string ClassName { get; set; }

    public RegistryKey? Parent { get; set; }

    /// <summary>
    ///     A unique value that can be used to find this key in a collection
    /// </summary>
    public string InternalGuid { get; set; }

    public KeyFlagsEnum KeyFlags { get; set; }

    /// <summary>
    ///     The name of this key. For the full path, see KeyPath
    /// </summary>
    public string KeyName => NkRecord.Name;

    /// <summary>
    ///     The full path to the  key, including its KeyName
    /// </summary>
    public string KeyPath
    {
        get
        {
            if (_keyPath != string.Empty)
            {
                //sometimes we have to update the path elsewhere, so if that happens, return it
                return _keyPath;
            }

            else if (Parent == null)
            {
                //This is the root key
                return $"{KeyName}";
            }
            else
            {
                return $@"{Parent.KeyPath}\{KeyName}";
            }
        }

        set => _keyPath = value;
    }

    /// <summary>
    ///     The last write time of this key
    /// </summary>
    public DateTimeOffset? LastWriteTime => NkRecord.LastWriteTimestamp;

    /// <summary>
    ///     The underlying NKRecord for this Key. This allows access to all info about the NK Record
    /// </summary>
    public NkCellRecord NkRecord { get; }

    /// <summary>
    ///     A list of child keys that exist under this key
    /// </summary>
    public List<RegistryKey> SubKeys { get; }

    /// <summary>
    ///     A list of values that exists under this key
    /// </summary>
    public List<KeyValue> Values { get; }
    #endregion
    #region Functions
    public string GetRegFormat(HiveTypeEnum hiveType)
    {
        var sb = new StringBuilder();

        string keyBase;

        switch (hiveType)
        {
            case HiveTypeEnum.NtUser:
                keyBase = "HKEY_CURRENT_USER";
                break;
            case HiveTypeEnum.Sam:
                keyBase = "HKEY_CURRENT_USER\\SAM";
                break;
            case HiveTypeEnum.Security:
                keyBase = "HKEY_CURRENT_USER\\SECURITY";
                break;
            case HiveTypeEnum.Software:
                keyBase = "HKEY_CURRENT_USER\\SOFTWARE";
                break;
            case HiveTypeEnum.System:
                keyBase = "HKEY_CURRENT_USER\\SYSTEM";
                break;
            case HiveTypeEnum.UsrClass:
                keyBase = "HKEY_CLASSES_ROOT";
                break;
            case HiveTypeEnum.Components:
                keyBase = "HKEY_CURRENT_USER\\COMPONENTS";
                break;
            case HiveTypeEnum.Amcache:
                keyBase = "";
                break;
            case HiveTypeEnum.Syscache:
                keyBase = "";
                break;

            default:
                keyBase = "HKEY_CURRENT_USER\\UNKNOWN_BASEPATH";
                break;
        }

        var keyNames = KeyPath.Split('\\');
        var normalizedKeyPath = string.Join("\\", keyNames.Skip(1));

        var keyName = normalizedKeyPath.Length > 0
            ? $"[{keyBase}\\{normalizedKeyPath}]"
            : $"[{keyBase}]";

        sb.AppendLine();
        sb.AppendLine(keyName);
        if (LastWriteTime.HasValue)
        {
            sb.AppendLine($";Last write timestamp {LastWriteTime.Value.UtcDateTime.ToString("o")}");
        }
        else
        {
            sb.AppendLine($";Last write timestamp unknown");
        }

        foreach (var keyValue in Values)
        {
            var keyNameOut = keyValue.ValueName;
            if (keyNameOut.ToLowerInvariant() == "(default)")
            {
                keyNameOut = "@";
            }
            else
            {
                keyNameOut = keyNameOut.Replace("\\", "\\\\");
                keyNameOut = $"\"{keyNameOut.Replace("\"", "\\\"")}\"";
            }

            var keyValueOut = string.Empty;

            switch (keyValue.VkRecord.DataType)
            {
                case VkCellRecord.DataTypeEnum.RegSz:
                    keyValueOut = $"\"{keyValue.ValueData.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
                    break;

                case VkCellRecord.DataTypeEnum.RegNone:
                case VkCellRecord.DataTypeEnum.RegDwordBigEndian:
                case VkCellRecord.DataTypeEnum.RegFullResourceDescription:
                case VkCellRecord.DataTypeEnum.RegMultiSz:
                case VkCellRecord.DataTypeEnum.RegQword:
                case VkCellRecord.DataTypeEnum.RegFileTime:
                case VkCellRecord.DataTypeEnum.RegLink:
                case VkCellRecord.DataTypeEnum.RegResourceRequirementsList:
                case VkCellRecord.DataTypeEnum.RegExpandSz:
                    var prefix = $"hex({(int)keyValue.VkRecord.DataType:x}):";
                    keyValueOut =
                        $"{prefix}{BitConverter.ToString(keyValue.ValueDataRaw).Replace("-", ",")}".ToLowerInvariant
                            ();
                    if (keyValueOut.Length + prefix.Length + keyNameOut.Length > 76)
                        keyValueOut =
                            $"{prefix}{FormatBinaryValueData(keyValue.ValueDataRaw, keyNameOut.Length, prefix.Length)}";

                    break;

                case VkCellRecord.DataTypeEnum.RegDword:
                    keyValueOut =
                        $"dword:{BitConverter.ToInt32(keyValue.ValueDataRaw, 0):X8}"
                            .ToLowerInvariant();
                    break;

                case VkCellRecord.DataTypeEnum.RegBinary:
                    keyValueOut =
                        $"hex:{BitConverter.ToString(keyValue.ValueDataRaw).Replace("-", ",")}"
                            .ToLowerInvariant();
                    if (keyValueOut.Length + 5 + keyNameOut.Length > 76)
                        keyValueOut = $"hex:{FormatBinaryValueData(keyValue.ValueDataRaw, keyNameOut.Length, 5)}";

                    break;
            }

            sb.AppendLine($"{keyNameOut}={keyValueOut}");
        }

        return sb.ToString().TrimEnd();
    }

    private string FormatBinaryValueData(byte[] data, int prefixLength, int nameLength)
    {
        //each line is 80 chars long max
        var tempkeyVal = new StringBuilder();

        int charsWritten;
        charsWritten = nameLength + prefixLength; //account for the name and whatever the hex prefix looks like

        var lineLength = charsWritten;

        var dataIndex = 0;

        while (dataIndex < data.Length)
        {
            tempkeyVal.Append($"{data[dataIndex]:x2},");
            dataIndex += 1;
            charsWritten += 3; //2 hex chars plus a comma
            lineLength += 3;

            if (lineLength >= 76)
            {
                tempkeyVal.AppendLine("\\");
                tempkeyVal.Append("  ");
                charsWritten += 2;
                lineLength = 2;
            }
        }

        var ret = tempkeyVal.ToString();
        ret = ret.Trim();

        ret = ret.TrimEnd('\\');
        ret = ret.TrimEnd(',');
        ret = ret.TrimEnd('\\');

        return ret.ToLowerInvariant();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Key Name: {KeyName}");
        sb.AppendLine($"Key Path: {KeyPath}");
        sb.AppendLine();

        sb.AppendLine($"Last Write Time: {LastWriteTime}");
        sb.AppendLine();

        sb.AppendLine($"Key flags: {KeyFlags}");

        sb.AppendLine();

        sb.AppendLine($"NK Record: {NkRecord}");

        sb.AppendLine();

        sb.AppendLine($"SubKey count: {SubKeys.Count:N0}");

        var i = 0;
        foreach (var sk in SubKeys)
        {
            sb.AppendLine($"------------ SubKey #{i} ------------");
            sb.AppendLine(sk.ToString());
            i += 1;
        }

        sb.AppendLine();

        sb.AppendLine($"Value count: {Values.Count:N0}");

        i = 0;
        foreach (var value in Values)
        {
            sb.AppendLine($"------------ Value #{i} ------------");
            sb.AppendLine(value.ToString());
            i += 1;
        }

        return sb.ToString();
    }
    public object GetValue(string keyName)
    {
        if (Values is not null)
        {
            KeyValue? kv = Values.FirstOrDefault(v => v.ValueName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
            if (kv is not null)
            {
                return kv.ValueData;
            }
        }
        return new();
    }
    public object GetValue(string keyName, object defaultValue)
    {
        return GetValue(keyName) ?? defaultValue;
    }
    #endregion
}