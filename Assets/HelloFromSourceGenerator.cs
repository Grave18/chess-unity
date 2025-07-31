using MvvmTool;
using UnityEngine;

public class HelloFromSourceGenerator : MonoBehaviour
{
    [ObservableProperty]
    private int lol;
    private static string GetStringFromSourceGenerator()
    {
        return "Hello from unity";
    }

    // Start is called before the first frame update
    private void Start()
    {
        var output = "Test";
        output = GetStringFromSourceGenerator();
        Debug.Log(output);
    }
}
