// FILE: IslandManager.cs
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IslandManager : MonoBehaviour {

	public void GenerateNewIsland(GeneratorSettings settings) {
		if (settings == null) {
			Debug.LogError("GeneratorSettings asset is null. Cannot generate island.");
			return;
		}

		DeleteAllChildren();

		GameObject islandObject = new GameObject();
		islandObject.transform.SetParent(this.transform);

		Terrain terrain = islandObject.AddComponent<Terrain>();
		terrain.terrainData = new TerrainData();

		IslandGenerator generator = new IslandGenerator(settings); // Use the passed-in settings
		ConfigureTerrainData(terrain.terrainData, settings);

		islandObject.name = $"Island (Seed: {generator.GetSeed()})";

		// --- FIX ---
		// Get the actual resolution from the terrain data after it has been configured and potentially clamped by Unity.
		int actualResolution = terrain.terrainData.heightmapResolution;
		generator.GenerateMaps(actualResolution, out float[,] heightMap, out float[,,] splatMap);
		// -----------

		terrain.terrainData.SetHeights(0, 0, heightMap);
		if (terrain.terrainData.alphamapLayers > 0) {
			terrain.terrainData.SetAlphamaps(0, 0, splatMap);
		}

		Debug.Log($"Successfully generated new island '{islandObject.name}'.", islandObject);
	}

	private void ConfigureTerrainData(TerrainData terrainData, GeneratorSettings settings) {
		terrainData.heightmapResolution = settings.heightmapResolution;
		terrainData.alphamapResolution = settings.heightmapResolution;
		terrainData.size = settings.terrainSize;
		terrainData.terrainLayers = settings.GenerateTerrainLayers();
	}

	public void DeleteAllChildren() {
		for (int i = transform.childCount - 1; i >= 0; i--) {
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
#else
			Destroy(transform.GetChild(i).gameObject);
#endif
		}
	}
}