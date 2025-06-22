csharp
using UnityEngine; // Correctly placed inside the global scope

public class TerrainGeneratorManager : MonoBehaviour // Class definition
{
    [Tooltip("The settings asset that defines the island to be generated.")]
    public GeneratorSettings settings; // Field
    [Tooltip("The IslandManager component in the scene.")]
    public IslandManager islandManager; // Field

    public void GenerateTerrain() // Method
    {
        if (settings == null)
        {
            Debug.LogError("Assign a GeneratorSettings asset to the TerrainGeneratorManager before generating.");
            return;
        }
        if (islandManager == null)
        {
            Debug.LogError("Assign the IslandManager component to the TerrainGeneratorManager before generating.");
            return;
        }
        islandManager.GenerateNewIsland(settings);
    }
}