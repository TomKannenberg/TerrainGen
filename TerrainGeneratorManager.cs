csharp
using UnityEngine;

public class TerrainGeneratorManager : MonoBehaviour
{
    [Tooltip("The settings asset that defines the island to be generated.")]
    public GeneratorSettings settings;
    [Tooltip("The IslandManager component in the scene.")]
    public IslandManager islandManager;

    public void GenerateTerrain()
    {
        if (settings == null)
        {
            Debug.LogError("Assign a GeneratorSettings asset to the TerrainGeneratorManager before generating.");
            return;
        }
        if (islandManager == null) {
            Debug.LogError("Assign the IslandManager component to the TerrainGeneratorManager before generating.");
            return;
        }
        islandManager.GenerateNewIsland(settings);
    }
}