using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GeneratorRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Modify this path to match your Unity project's script folder
                string unityScriptsPath = args.FirstOrDefault()
                                          ?? @"D:\dev\unity\portfolio\chess_3d\Assets\Code";

                if (!Directory.Exists(unityScriptsPath))
                {
                    Console.WriteLine($"Directory does not exist: {unityScriptsPath}");
                }
                else
                {
                    var sourceFiles = Directory.GetFiles(unityScriptsPath, "*.cs", SearchOption.AllDirectories);

                    var syntaxTrees = sourceFiles.Select(file =>
                    {
                        var sourceText = File.ReadAllText(file);
                        return CSharpSyntaxTree.ParseText(sourceText, path: file);
                    }).ToList();

                    // Add basic references - expand as needed
                    var references = new List<MetadataReference>
                    {
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location),
                    };

                    // Create a Roslyn compilation
                    var compilation = CSharpCompilation.Create(
                        assemblyName: "UnityAssembly",
                        syntaxTrees: syntaxTrees,
                        references: references,
                        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                    // Create the generator instance
                    var generator = new ObservablePropertyGenerator(); // Your custom ISourceGenerator

                    GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

                    // Run the generator
                    driver = driver.RunGeneratorsAndUpdateCompilation(
                        compilation,
                        out Compilation updatedCompilation,
                        out var diagnostics,
                        CancellationToken.None);

                    // Get results
                    GeneratorDriverRunResult runResult = driver.GetRunResult();

                    var outputDir = Path.Combine(unityScriptsPath, "Generated");
                    Directory.CreateDirectory(outputDir);

                    foreach (GeneratorRunResult result in runResult.Results)
                    {
                        foreach (GeneratedSourceResult generatedSource in result.GeneratedSources)
                        {
                            string filename = generatedSource.HintName;
                            string code = generatedSource.SourceText.ToString();
                            string path = Path.Combine(outputDir, filename);

                            File.WriteAllText(path, code);
                            Console.WriteLine($"Generated: {filename}");
                        }
                    }

                    Console.WriteLine("Done.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Press Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
