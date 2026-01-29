#region Usings
using System.Text;
using CODA.RegistryParser.Cells;
#endregion

namespace CODA.RegistryParser.Abstractions;
/// <summary>
///     Represents a value that is associated with a RegistryKey
///     <remarks>Also contains references to low level structures related to a given value</remarks>
/// </summary>
public class KeyValue
{
    #region Constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyValue" /> class.
    /// </summary>
    public KeyValue(VkCellRecord vk)
    {
        VkRecord = vk;
        InternalGuid = Guid.NewGuid().ToString();
    }
    #endregion
    #region Properties
    /// <summary>
    ///     A unique value that can be used to find this key in a collection
    /// </summary>
    public string InternalGuid { get; }

    /// <summary>
    ///     The normalized representation of the value's value.
    /// </summary>
    public string ValueData
    {
        get
        {
            if (VkRecord.ValueData is not null)
            {

                if (VkRecord.ValueData is byte[])
                {
                    return BitConverter.ToString((byte[])VkRecord.ValueData);
                }
                else
                {
                    return VkRecord.ValueData.ToString() ?? string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    ///     The value as stored in the hive as a series of bytes
    /// </summary>
    public byte[] ValueDataRaw => VkRecord.ValueDataRaw;

    public string ValueName => VkRecord.ValueName;

    /// <summary>
    ///     If present, the value slack as a string of bytes delimited by hyphens
    /// </summary>
    public string ValueSlack => BitConverter.ToString(VkRecord.ValueDataSlack);

    /// <summary>
    ///     The value slack as stored in the hive as a series of bytes
    /// </summary>
    public byte[] ValueSlackRaw => VkRecord.ValueDataSlack;

    /// <summary>
    ///     The values type (VKCellRecord.DataTypeEnum)
    /// </summary>
    public string ValueType => VkRecord.DataType.ToString();

    /// <summary>
    ///     The underlying VKRecord for this Key. This allows access to all info about the VK Record
    /// </summary>
    public VkCellRecord VkRecord { get; }
    #endregion
    #region Functions
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"VK Record: {VkRecord}");
        return sb.ToString();
    }
    #endregion
}