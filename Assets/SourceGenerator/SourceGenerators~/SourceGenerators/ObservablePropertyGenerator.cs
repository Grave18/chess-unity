using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

public class ObservablePropertyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        // Iterate over all syntax trees in the compilation
        foreach (SyntaxTree tree in context.Compilation.SyntaxTrees)
        {
            SyntaxNode root = tree.GetRoot();
            var fieldsWithAttribute = root
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(f => f.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(attr => attr.Name.ToString().Contains("ObservableProperty")));


            foreach (FieldDeclarationSyntax field in fieldsWithAttribute)
            {
                VariableDeclaratorSyntax variable = field.Declaration.Variables.First();
                var fieldName = variable.Identifier.Text;
                var propertyName = char.ToUpper(fieldName[1]) + fieldName.Substring(2); // from _foo to Foo

                ClassDeclarationSyntax classDecl = field.Ancestors().OfType<ClassDeclarationSyntax>().First();
                var className = classDecl.Identifier.Text;

                var generatedCode = $$"""
                                      // Generated code
                                      namespace {{GetNamespace(classDecl)}}
                                      {
                                          public partial class {{className}}
                                          {
                                              public bool {{propertyName}}
                                              {
                                                  get => {{fieldName}};
                                                  set => SetField(ref {{fieldName}}, value);
                                              }
                                          }
                                      }

                                      """;

                context.AddSource($"{className}_{propertyName}_Generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
            }
        }
    }

    private static string GetNamespace(SyntaxNode node)
    {
        return node.Ancestors()
            .OfType<NamespaceDeclarationSyntax>()
            .FirstOrDefault()?.Name.ToString() ?? "GlobalNamespace";
    }
}