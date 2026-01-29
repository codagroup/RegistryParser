namespace CODA.RegistryParser.Test;

internal static class TestHelpers
{
    internal static string HivePath {get; set;} = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/hives";
}