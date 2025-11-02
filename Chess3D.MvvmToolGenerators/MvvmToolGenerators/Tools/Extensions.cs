using System.Linq;
using Microsoft.CodeAnalysis;

namespace MvvmToolGenerators.Tools;

public static class Extensions
{
    public static void ShowDiagnostics(this GeneratorExecutionContext context,
        string id,
        string message,
        DiagnosticSeverity severity = DiagnosticSeverity.Info,
        params object[] args)
    {
        var descriptor = new DiagnosticDescriptor(
            id: id,
            title: "Source Generator Debug",
            messageFormat: message,
            category: "Generator",
            severity,
            isEnabledByDefault: true
        );

        var diagnostic = Diagnostic.Create(descriptor, Location.None, args);
        context.ReportDiagnostic(diagnostic);
    }

    public static bool IsUnityAndNotAssemblyCsharp(this Compilation compilation)
    {
        // Hack to avoid generating code for non Assembly-CSharp assemblies
        bool isUnity = IsUnity(compilation);
        bool isNotProjectRuntimeAssembly = compilation.AssemblyName != ProjectConstants.ProjectRuntimeAssemblyName;

        return isUnity && isNotProjectRuntimeAssembly;

        return true;
    }

    public static bool IsUnity(this Compilation compilation)
    {
        bool isUnity = compilation.ReferencedAssemblyNames
            .Any(a => a.Name == "UnityEngine");

        return isUnity;
    }
}