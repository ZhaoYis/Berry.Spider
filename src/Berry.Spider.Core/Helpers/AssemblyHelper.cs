using System.Reflection;

namespace Berry.Spider.Core;

public static class AssemblyHelper
{
    private static readonly List<Type> ExportedTypes = new List<Type>();

    static AssemblyHelper()
    {
        LoadCurrentDomainExportedTypes();
    }

    public static List<Type> CurrentDomainExportedTypes => ExportedTypes;

    private static void LoadCurrentDomainExportedTypes()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies.Where(c => c.FullName != null && c.FullName.StartsWith("Berry")))
        {
            ExportedTypes.AddRange(assembly.ExportedTypes);
        }
    }
}