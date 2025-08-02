using System.ComponentModel;
using MvvmTool;

namespace SourceGenerators.Sample;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Console.ReadLine();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}